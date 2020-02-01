/*
 * Copyright 2019 STEM Management
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 */

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using STEM.Sys.IO.TCP;

namespace STEM.Sys.Messaging
{
    
    [XmlType(TypeName = "STEM.Sys.Messaging.Message")]
    public class Message : STEM.Sys.Serializable, IDisposable
    {        
        /// <summary>
        /// A uniqueue ID used to track this message
        /// </summary>
        [DisplayName("Message ID"), ReadOnlyAttribute(true)]
        public Guid MessageID { get; set; }

        /// <summary>
        /// Gets the UTC DateTime that this message was sent
        /// </summary>
        [DisplayName("Time Sent"), ReadOnlyAttribute(true)]
        public DateTime TimeSent { get; set; }

        /// <summary>
        /// Gets the UTC DateTime that this message was received
        /// </summary>
        [DisplayName("Time Received"), ReadOnlyAttribute(true)]
        public DateTime TimeReceived { get; set; }
        
        object _ObjectLock = new object();

        public bool Compress { get; set; }

        public Message()
        {
            MessageID = Guid.NewGuid();
            Compress = true;
            AcceptResponsesUntilDisposed = false;
            SentVia = new List<MessageConnection>();
        }

        [XmlIgnore]
        public MessageConnection MessageConnection { internal set; get; }
        
        public string ReceivedAtAddress()
        {
            if (MessageConnection != null)
                return MessageConnection.LocalAddress;

            return STEM.Sys.IO.Net.EmptyAddress;
        }

        public string SentFromAddress()
        {
            if (MessageConnection != null)
                return MessageConnection.RemoteAddress;

            return STEM.Sys.IO.Net.EmptyAddress;
        }

        public int ReceivedOnPort()
        {
            if (MessageConnection != null)
                return MessageConnection.LocalPort;

            return -1;
        }

        public int SentFromPort()
        {
            if (MessageConnection != null)
                return MessageConnection.RemotePort;

            return -1;
        }

        public virtual bool Respond(Message message)
        {
            if (MessageConnection != null)
                return MessageConnection.Send(new Response(this, message));
            else
                return false;
        }

        public virtual Message Respond(Message message, TimeSpan waitForResponse)
        {
            if (MessageConnection != null)
                return MessageConnection.Send(new Response(this, message), waitForResponse);
            else
                return new Undeliverable();
        }

        /// <summary>
        /// Receive a message that was sent in response to this message
        /// </summary>
        public delegate void MessageResponse(Message delivered, Message response);

        internal List<MessageResponse> _onResponse = new List<MessageResponse>();

        /// <summary>
        /// Receive a message that was sent in response to this message
        /// </summary>
        public event MessageResponse onResponse
        {
            add
            {
                if (value == null)
                    return;

                lock (_onResponse)
                    if (!_onResponse.Contains(value))
                    {
                        _onResponse.Add(value);
                        AcceptingResponses = true;
                    }
            }

            remove
            {
                if (value == null)
                    return;

                lock (_onResponse)
                {
                    if (_onResponse.Contains(value))
                    {
                        _onResponse.Remove(value);
                    }

                    if (_onResponse.Count == 0)
                    {
                        AcceptingResponses = false;

                        lock (SentVia)
                            foreach (MessageConnection c in SentVia)
                                c.StopWaiting(this);
                    }
                }
            }
        }

        [XmlIgnore]
        public List<MessageConnection> SentVia { get; private set; }
        internal Queue<Message> Responses = new Queue<Message>();
        internal bool AcceptingResponses { get; set; }
        public bool AcceptResponsesUntilDisposed { get; set; }

        internal void RegisterSender(MessageConnection sender)
        {
            lock (SentVia)
                if (!SentVia.Contains(sender))
                    SentVia.Add(sender);
        }

        /// <summary>
        /// Get the next response to this message
        /// </summary>
        /// <param name="wait">The time to wait for a response to arrive if none are queued at this time</param>
        /// <returns>A message response or null if none have arrived</returns>
        public Message GetNextResponse(TimeSpan wait)
        {
            DateTime start = DateTime.UtcNow;
            while ((DateTime.UtcNow - start).Ticks < wait.Ticks)
            {
                lock (Responses)
                    if (Responses.Count > 0)
                        return Responses.Dequeue();

                System.Threading.Thread.Sleep(10);
            }

            return null;
        }

        /// <summary>
        /// Get the next response to this message
        /// </summary>
        /// <returns>A message response or null if none have arrived</returns>
        public Message GetNextResponse()
        {
            lock (Responses)
                if (Responses.Count > 0)
                    return Responses.Dequeue();

            return null;
        }

        public bool ReceiveResponse(Message response)
        {
            bool handled = false;
            try
            {
                List<MessageResponse> call = null;
                lock (_onResponse)
                    call = _onResponse.ToList();

                foreach (MessageResponse c in call)
                    try
                    {
                        c(this, response);
                        handled = true;
                    }
                    catch (Exception ex)
                    {
                        STEM.Sys.EventLog.WriteEntry("Message.onResponse", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                    }

                if (!handled)
                {
                    lock (Responses)
                        Responses.Enqueue(response);

                    if (!AcceptResponsesUntilDisposed)
                    {
                        lock (_onResponse)
                        {
                            lock (SentVia)
                                foreach (MessageConnection c in SentVia)
                                    c.StopWaiting(this);
                        }
                    }
                    else
                    {
                        handled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                STEM.Sys.EventLog.WriteEntry("Message.ReceiveResponse", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
            }

            return handled;
        }
        
        public void Dispose()
        {
            try
            {
                Dispose(true);
            }
            catch { }

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            try
            {
                List<EventHandler> call = null;
                lock (_onDisposing)
                    call = _onDisposing.ToList();

                foreach (EventHandler c in call)
                    try
                    {
                        c(this, EventArgs.Empty);
                    }
                    catch (Exception ex)
                    {
                        STEM.Sys.EventLog.WriteEntry("Message.Disposing", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                    }
            }
            catch (Exception ex)
            {
                STEM.Sys.EventLog.WriteEntry("Message.Dispose", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
            }

            lock (_onResponse)
            {
                AcceptingResponses = false;
                AcceptResponsesUntilDisposed = false;

                _onResponse = new List<MessageResponse>();

                lock (SentVia)
                    foreach (MessageConnection c in SentVia)
                        c.StopWaiting(this);
            }
        }

        List<EventHandler> _onDisposing = new List<EventHandler>();
        public event EventHandler onDisposing
        {

            add
            {
                if (value == null)
                    return;

                lock (_onDisposing)
                    if (!_onDisposing.Contains(value))
                        _onDisposing.Add(value);
            }

            remove
            {
                if (value == null)
                    return;

                lock (_onDisposing)
                    if (_onDisposing.Contains(value))
                        _onDisposing.Remove(value);
            }
        }

        public virtual void CopyFrom(Message source)
        {
            if (source != null)
                CopyFrom(source, source.MessageID);
        }

        public virtual void CopyFrom(Message source, Guid destinationMessageID)
        {
            if (source != null)
                lock (source._ObjectLock)
                    lock (_ObjectLock)
                    {
                        if (object.ReferenceEquals(source, this))
                            return;

                        this.MessageID = destinationMessageID;
                        this.TimeSent = source.TimeSent;
                        this.TimeReceived = source.TimeReceived;
                        this.MessageConnection = source.MessageConnection;

                        if (this._onResponse.Count > 0)
                            foreach (MessageResponse x in this._onResponse)
                                this.onResponse -= x;

                        if (source._onResponse.Count > 0)
                            foreach (MessageResponse x in source._onResponse)
                                this.onResponse += x;

                        this.AcceptResponsesUntilDisposed = source.AcceptResponsesUntilDisposed;
                    }
        }
    }
}
