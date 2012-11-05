using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;

namespace AppFitbitUltraSecurity
{
    [System.AddIn.AddIn("AppFitbitUltraSecurity")]
    public class AppFitbitUltraSecurity : ModuleBase
    {
        LogWindow logWindow;

        #region Fitbit Ultra Variables

        View.VPort fitbitPort = null;
        View.VCapability fitbitPortCapability = null;

        #endregion

        #region ZWave Variables

        View.VPort sensorPort = null;
        View.VCapability sensorPortCapability = null;

        #endregion        

        bool homeSecurityActivated = false;

        public override void Start()
        {
            logger.Log("Starting {0}", ToString());

            IList<View.VPort> allPortsList = GetAllPortsFromPlatform();
            foreach (View.VPort port in allPortsList)
            {
                PortRegistered(port);
            }

            ShowWindow();

            Work();
        }

        public void ShowWindow()
        {
            SafeThread thread = new SafeThread(
                delegate()
                {
                    logWindow = new LogWindow(this, logger, moduleInfo.FriendlyName());
                    logWindow.ShowDialog();
                },
                "Launching window for " + ToString(),
                logger);

            thread.SetApartmentState(System.Threading.ApartmentState.STA);
            thread.Start();
        }

        public override void Stop()
        {
            if (logWindow != null) 
                 logWindow.Invoke(new Action(delegate() { logWindow.Close(); }));

            Finished();
        }

        public void Work()
        {
            int counter = 0;
            while (true)
            {
                counter++;

                lock (this)
                {
                    if (logWindow != null)
                        logWindow.Invoke(new Action(delegate() { logWindow.OtherPortsLog(String.Format("{0} active Fitbit Ultra ports", 0)); }));

                    SendGetDevicePresenceMethod();
                    SendGetSwitchStatusMethod();
                   
                }

                System.Threading.Thread.Sleep(1 * 30 * 1000); // sleep 30 seconds
            }
        }

        #region Driver Methods

        public void SendGetDevicePresenceMethod()
        {
            if (fitbitPort != null && fitbitPortCapability == null)
            {
                fitbitPortCapability = GetCapability(fitbitPort, Globals.UserSystem);
            }

            if (fitbitPort != null && fitbitPortCapability != null)
            {
                IList<View.VParamType> retVals;

                try
                {
                    IList<View.VParamType> parameters = new List<View.VParamType>();
                    parameters.Add(new ParamType(ParamType.SimpleType.binary, "", DateTime.Today, "activityDate"));

                    retVals = fitbitPort.Invoke(RoleFitbitUltra.RoleName, RoleFitbitUltra.OpGetDevicePresence, parameters, ControlPort, fitbitPortCapability, ControlPortCapability);
                }
                catch (Exception e)
                {
                    logger.Log("Error while calling {0} request: {1}", RoleFitbitUltra.OpGetDevicePresence, e.ToString());
                    return;
                }

                bool result;

                string message = "";
                if (retVals[0].Maintype() != (int)ParamType.SimpleType.error)
                {
                    result = Convert.ToBoolean(retVals[0].Value());
                    if (result == true)
                    {
                        message = String.Format("User is home, and the home security system is {0}", homeSecurityActivated);
                        if (homeSecurityActivated)
                        {
                            message = String.Format("{0}\nDeactivating Home security system", message);
                            homeSecurityActivated = false;
                        }
                    }
                    else
                    {
                        message = String.Format("User is Away and the home security system is {0}", homeSecurityActivated);
                        if (!homeSecurityActivated)
                        {
                            message = String.Format("{0}\nActivating Home security system", message);
                            homeSecurityActivated = true;
                        }
                    }
                } 
                else
                {
                    message = String.Format("{0} failure to {1}", RoleFitbitUltra.OpGetDevicePresence, fitbitPort.ToString());
                    return;
                }
                
                LogMessageToWindow(message);
            }
        }

        public int SendGetSwitchStatusMethod()
        {
            if (sensorPort != null && sensorPortCapability == null)
            {
                sensorPortCapability = GetCapability(sensorPort, Globals.UserSystem);
            }

            if (sensorPort != null && sensorPortCapability != null)
            {
                IList<View.VParamType> retVals = null;

                try
                {
                    IList<View.VParamType> parameters = new List<View.VParamType>();                    
                    //retVals = sensorPort.Invoke(RoleSwitchBinary.RoleName, RoleSwitchBinary.OpGetName, parameters, ControlPort, sensorPortCapability, ControlPortCapability);
                    retVals = sensorPort.Invoke(RoleSensor.RoleName, RoleSensor.OpGetName, parameters, ControlPort, sensorPortCapability, ControlPortCapability);
                }
                catch (Exception e)
                {
                    logger.Log("Error while calling {0} request: {1}", RoleSensor.OpGetName, e.ToString());
                }

                int result = 0;

                string message = "";
                if (retVals[0].Maintype() != (int)ParamType.SimpleType.error)
                {
                    result = Convert.ToInt32(retVals[0].Value());
                    message = String.Format("{0} success to {1} result = {2}", RoleSensor.OpGetName, sensorPort.ToString(), result);
                }
                else
                {
                    message = String.Format("{0} failure to {1}", RoleSensor.OpGetName, sensorPort.ToString());
                }

                LogMessageToWindow(message);

                return result;
            }

            return -1;
        }

        public void LogMessageToWindow(string message)
        {
            logger.Log("{0} {1}", this.ToString(), message);

            if (logWindow != null)
                logWindow.Invoke(new Action(delegate() { logWindow.ConsoleLog(message); }));
        }

        #endregion

        #region Port Management


        public override void PortRegistered(View.VPort port)
        {
            logger.Log("{0} got registeration notification for {1}", ToString(), port.ToString());

            lock (this)
            {
                if (Role.ContainsRole(port, RoleFitbitUltra.RoleName))
                {
                    fitbitPort = port;
                    logger.Log("{0} added fitbitUltra port {1}", this.ToString(), port.ToString());

                    if (logWindow != null)
                        logWindow.Invoke(new Action(delegate() { logWindow.ConsoleLog("added port " + port.ToString()); }));
                }

                if (Role.ContainsRole(port, RoleSensor.RoleName))
                {
                    sensorPort = port;
                    logger.Log("{0} added switchbinary port {0}", this.ToString(), port.ToString());

                    if (logWindow != null)
                        logWindow.Invoke(new Action(delegate() { logWindow.ConsoleLog("added port " + port.ToString()); }));
                }
            }
        }

        public override void PortDeregistered(View.VPort port)
        {
            lock (this)
            {
                if (port.Equals(fitbitPort))
                {
                    fitbitPort = null;
                    logger.Log("{0} removed fitbitUltra port {1}", this.ToString(), port.ToString());

                    if (logWindow != null)
                        logWindow.Invoke(new Action(delegate() { logWindow.ConsoleLog("removed port " + port.ToString()); }));
                }

                if (port.Equals(sensorPort))
                {
                    sensorPort = null;
                    logger.Log("{0} removed switchbinary port {0}", this.ToString(), port.ToString());

                    if (logWindow != null)
                        logWindow.Invoke(new Action(delegate() { logWindow.ConsoleLog("removed port " + port.ToString()); }));
                }
            }
        }

        #endregion
    }
}