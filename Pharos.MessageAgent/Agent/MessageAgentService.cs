﻿using Pharos.SuperSocketProtocol;
using Pharos.MessageAgent.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Pharos.MessageAgent.Data;

namespace Pharos.MessageAgent.Agent
{
    /// <summary>
    /// 消息代理服务提供者
    /// </summary>
    public class MessageAgentService : IMessageAgentService
    {

        readonly byte[] pulishRouteCode = new byte[] { 0x01, 0x00, 0x00, 0x01 };
        readonly string pulishWebRouteCode = "Multiflora/MessageTransferAgent";
        public MessageAgentService(MessageServer server)
        {
            Server = server;
        }

        #region IMessageAgentService
        public MessageServer Server { get; set; }
        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="info"></param>
        /// <param name="sessionId"></param>
        public void Subscribe(SubscribeInformaction info, string sessionId)
        {
            var subscribe = Server.MessageStore.GetSubscribe(info.SubscribeId, info.Topic);
            var record = new MessageSubscribeRecord() { CurrentSessionId = sessionId, SubscribeInformaction = info };
            if (subscribe == null)
            {
                //先查询是否存在同主题订阅，不存在则向消息队列订阅
                if (!Server.MessageStore.HasTopicSubscribe(info.Topic))
                {
                    Server.MessageQueue.Subscribe(info);
                }
                Server.MessageStore.SaveSubscribe(record);
            }
            else
            {
                Server.MessageStore.UpdateSubscribe(record);
            }
            //异步处理失败推送重试
            Task.Factory.StartNew((o) =>
            {
                var subscribeRecord = (MessageSubscribeRecord)o;
                RetryFailedSubscription(subscribeRecord.SubscribeInformaction, subscribeRecord.CurrentSessionId);
            }, record);
        }
        /// <summary>
        ///取消订阅
        /// </summary>
        /// <param name="info"></param>
        public void UnSubscribe(SubscribeInformaction info)
        {
            Server.MessageStore.RemoveSubscribe(info.SubscribeId, info.Topic);
            //删除后查询是否存在同主题订阅，不存在则向消息队列取消订阅
            if (!Server.MessageStore.HasTopicSubscribe(info.Topic))
            {
                Server.MessageQueue.Subscribe(info);
            }
        }
        /// <summary>
        /// 推送给消息队列
        /// </summary>
        /// <param name="info"></param>
        public void Pubish(PubishInformaction info)
        {
            Server.MessageQueue.Pubish(info);
        }
        /// <summary>
        /// 处理接收到的推送消息，并推送给当前代理的订阅方
        /// </summary>
        /// <param name="info"></param>
        public void ReceiveMessage(PubishInformaction info)
        {
            IEnumerable<MessageSubscribeRecord> topicSubscribers = Server.MessageStore.GetSubscribes(info.Topic);

            if (topicSubscribers == null || topicSubscribers.Count() == 0)
                return;

            List<MessageSubscribeRecord> errorPubishRecords = new List<MessageSubscribeRecord>();
            var isSuccess = false;
            foreach (var item in topicSubscribers)
            {
                try
                {
                    //优先进行socket推送
                    isSuccess = SocketPubish(item.SubscribeInformaction, info, item.CurrentSessionId);

                    //使用Socket推送，如果失败检查是否支持使用Web推送
                    if (!isSuccess && item.SubscribeInformaction.IsWebSiteSubscriber)
                    {
                        isSuccess = WebPubish(item.SubscribeInformaction, info);
                    }
                }
                catch (Exception)
                {
                    isSuccess = false;
                }
                if (!isSuccess)
                {
                    errorPubishRecords.Add(item);
                }
            }
            Server.MessageStore.SaveFailureRecords(info, errorPubishRecords);
        }
        #endregion IMessageAgentService

        #region private method
        /// <summary>
        /// 失败订阅重试
        /// </summary>
        /// <param name="info"></param>
        /// <param name="session"></param>
        private void RetryFailedSubscription(SubscribeInformaction info, string session)
        {
            var counter = 0;
            while (true)
            {
                var msg = Server.MessageStore.GetAndRemoveFailureRecord(info.SubscribeId, info.Topic);
                if (msg == null) return;
                var isSuccess = SocketPubish(info, msg, session);

                if (!isSuccess && info.IsWebSiteSubscriber)
                {
                    WebPubish(info, msg);
                }
                counter++;
                if (counter > 500)
                {
                    counter = 0;
                    Thread.Sleep(100);//重试推送，防止CPU长时间占用
                }
            }
        }
        #endregion private method

        #region PubishMethod
        public bool WebPubish(SubscribeInformaction subInfo, PubishInformaction pubInfo)
        {
            if (!subInfo.IsWebSiteSubscriber) throw new Exception("Web推送不支持非Web应用!");
            try
            {
                var url = new Uri(new UriBuilder(subInfo.WebSiteURI).Uri, pulishWebRouteCode);
                var content = url.PostJson(pubInfo);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool SocketPubish(SubscribeInformaction subInfo, PubishInformaction pubInfo, string sessionId)
        {
            var session = Server.GetSessionByID(sessionId);
            if (session != null && session.Status == SessionStatus.Started && session.Connected)
            {
                try
                {
                    var topicBytes = session.TextToBytes(pubInfo.Topic);
                    var contentBytes = session.TextToBytes(pubInfo.Content);
                    var lenBytes = BitConverter.GetBytes(topicBytes.Length);

                    var body = new byte[topicBytes.Length + contentBytes.Length + 4];
                    Array.Copy(lenBytes, 0, body, 0, 4);
                    Array.Copy(topicBytes, 0, body, 4, topicBytes.Length);
                    Array.Copy(contentBytes, 0, body, 4 + topicBytes.Length, contentBytes.Length);
                    session.SendBytes(pulishRouteCode, body);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        #endregion PubishMethod

    }
}