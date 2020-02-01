///*
// * Copyright 2019 STEM Management
// *
// * Licensed under the Apache License, Version 2.0 (the "License");
// * you may not use this file except in compliance with the License.
// * You may obtain a copy of the License at
// *
// *   http://www.apache.org/licenses/LICENSE-2.0
// *
// * Unless required by applicable law or agreed to in writing, software
// * distributed under the License is distributed on an "AS IS" BASIS,
// * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// * See the License for the specific language governing permissions and
// * limitations under the License.
// * 
// */

//using System;
//using System.ComponentModel;
//using STEM.Surge;

// FIXME
//namespace STEM.Surge.Machine
//{
//    [TypeConverter(typeof(ExpandableObjectConverter))]
//    [DisplayName("ServiceController")]
//    [Description("Stop or Start a service named 'Service Name' on 'Machine IP'.")]
//    public class ServiceController : Instruction
//    {
//        public enum ServiceControllerAction {Start, Stop};

//        [DisplayName("Service Name"), DescriptionAttribute("What is the executing service name?")]
//        public string ServiceName { get; set; }
//        [DisplayName("Action"), DescriptionAttribute("What would you like the service to do (Start/Stop)?")]
//        public ServiceControllerAction Action { get; set; }
//        [DisplayName("Number of retries"), DescriptionAttribute("How many times should each operation be attempted?")]
//        public int Retry { get; set; }
//        [DisplayName("Seconds between retries"), DescriptionAttribute("How many seconds should be waited between retries?")]
//        public double RetryDelaySeconds { get; set; }

//        public ServiceController() : base()
//        {
//            Retry = 1;
//            RetryDelaySeconds = 2;
//            ServiceName = "ServiceName";
//        }

//        public ServiceController(string serviceName, int retry, double retryDelaySeconds)
//            : base()
//        {
//            ServiceName = serviceName;
//            Retry = retry;
//            RetryDelaySeconds = retryDelaySeconds;
//        }

//        protected override bool _Run()
//        {
//            int retry = 0;
//            while (retry++ <= Retry && !Stop)
//            {
//                System.ServiceProcess.ServiceController serviceController = null;
//                try
//                {
//                    serviceController = new System.ServiceProcess.ServiceController(ServiceName, ip);

//                    if (Action == ServiceControllerAction.Start)
//                    {
//                        if (serviceController.Status != ServiceControllerStatus.Running)
//                            serviceController.Start();

//                        int r2 = 0;
//                        while (serviceController.Status != ServiceControllerStatus.Running && r2++ <= 5)
//                            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(RetryDelaySeconds));

//                        Message += ServiceName + " on " + ip + " started.\r\n";
//                    }
//                    else
//                    {
//                        if (serviceController.Status == ServiceControllerStatus.Running)
//                            serviceController.Stop();

//                        int r2 = 0;
//                        while (serviceController.Status != ServiceControllerStatus.Stopped && r2++ <= 5)
//                            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(RetryDelaySeconds));

//                        Message += ServiceName + " on " + ip + " stopped.\r\n";
//                    }

//                    break;
//                }
//                catch (Exception ex)
//                {
//                    if (retry >= Retry)
//                    {
//                        Exceptions.Add(ex);
//                        if (Action == ServiceControllerAction.Start)
//                            Message += ServiceName + " on " + ip + " not started.\r\n";
//                        else
//                            Message += ServiceName + " on " + ip + " not stopped.\r\n";

//                        break;
//                    }
//                    else
//                    {
//                        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(RetryDelaySeconds));
//                    }
//                }
//                finally
//                {
//                    if (serviceController != null)
//                        serviceController.Close();
//                }
//            }

//            return (Exceptions.Count == 0);
//        }
        
//        protected override void _Rollback()
//        {
//                int retry = 0;
//            while (retry++ <= Retry && !Stop)
//            {
//                System.ServiceProcess.ServiceController serviceController = null;
//                try
//                {
//                    serviceController = new System.ServiceProcess.ServiceController(ServiceName, ip);

//                    if (Action == ServiceControllerAction.Start)
//                    {
//                        if (serviceController.Status == ServiceControllerStatus.Running)
//                            serviceController.Stop();

//                        int r2 = 0;
//                        while (serviceController.Status != ServiceControllerStatus.Stopped && r2++ <= 5)
//                            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(RetryDelaySeconds));

//                        Message += ServiceName + " on " + ip + " stopped.\r\n";
//                    }
//                    else
//                    {
//                        if (serviceController.Status != ServiceControllerStatus.Running)
//                            serviceController.Start();

//                        int r2 = 0;
//                        while (serviceController.Status != ServiceControllerStatus.Running && r2++ <= 5)
//                            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(RetryDelaySeconds));

//                        Message += ServiceName + " on " + ip + " started.\r\n";
//                    }

//                    break;
//                }
//                catch (Exception ex)
//                {
//                    if (retry >= Retry)
//                    {
//                        Exceptions.Add(ex);
//                        break;
//                    }
//                    else
//                    {
//                        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(RetryDelaySeconds));
//                    }
//                }
//                finally
//                {
//                    if (serviceController != null)
//                        serviceController.Close();
//                }
//            }
//        }
//    }
//}
