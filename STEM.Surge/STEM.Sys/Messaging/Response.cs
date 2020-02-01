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
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace STEM.Sys.Messaging
{
    public class Response : Message
    {
        object _ObjectLock = new object();

        public Response()
            : base()
        {
        }

        public Response(Message respondingTo, Message response)
            : base()
        {
            if (respondingTo == null)
                throw new ArgumentNullException(nameof(respondingTo));

            if (response == null)
                throw new ArgumentNullException(nameof(response));

            RespondingTo = respondingTo.MessageID;
            Compress = response.Compress;
            _Response = response;
        }
        
        /// <summary>
        /// A uniqueue ID used to track back to the orignal message
        /// </summary>
        [DisplayName("Orignal Message ID"), ReadOnlyAttribute(true)]
        public Guid RespondingTo { get; set; }

        Message _Response = null;

        [XmlIgnore]
        public Message Message
        {
            get
            {
                if (_Response == null)
                    lock (_ObjectLock)
                        if (_Response == null)
                        {
                            try
                            {
                                if (_ResponseXml != null)
                                    Message = Message.Deserialize(_ResponseXml) as Message;
                            }
                            catch (Exception ex)
                            {
                                STEM.Sys.EventLog.WriteEntry("Response:get", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                                _Response = null;
                            }
                        }

                return _Response;
            }

            set
            {
                lock (_ObjectLock)
                {
                    _Response = value;
                    _ResponseXml = null;
                }
            }
        }

        string _ResponseXml = null;
        [Browsable(false)]
        public string ResponseXml
        {
            get
            {
                lock (_ObjectLock)
                {
                    if (_ResponseXml == null && _Response != null)
                        ResponseXml = _Response.Serialize();

                    return _ResponseXml;
                }
            }

            set
            {
                lock (_ObjectLock)
                {
                    _ResponseXml = value;
                    _Response = null;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _ResponseXml = null;
            _Response = null;
        }
    }
}
