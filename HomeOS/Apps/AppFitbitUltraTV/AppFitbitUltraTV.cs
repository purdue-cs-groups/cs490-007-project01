using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;

namespace AppFitbitUltraTV
{
    [System.AddIn.AddIn("AppFitbitUltraTV")]
    public class AppFitbitUltraTV : ModuleBase
    {
        #region Fitbit Ultra Variables

        View.VPort fitbitPort = null;
        View.VCapability fitbitPortCapability = null;

        #endregion

        #region ZWave Variables

        View.VPort switchPort = null;
        View.VCapability switchPortCapability = null;

        #endregion

        LogWindow logWindow = null;

        int currentSteps = 0;

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
                        logWindow.Invoke(new Action(delegate() { logWindow.OtherPortsLog(String.Format("{0} active Fitbit Ultra ports", 1)); }));

                    SendGetDeviceCurrentStepsMethod(counter);
                    SendGetDeviceCurrentStepsGoalMethod(counter);
                }

                System.Threading.Thread.Sleep(1 * 30 * 1000); // sleep 30 seconds
            }
        }

        #region Driver Methods

        public void SendGetDeviceCurrentStepsMethod(int counter)
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

                    retVals = fitbitPort.Invoke(RoleFitbitUltra.RoleName, RoleFitbitUltra.OpGetSteps, parameters, ControlPort, fitbitPortCapability, ControlPortCapability);
                }
                catch (Exception e)
                {
                    logger.Log("Error while calling {0} request: {1}", RoleFitbitUltra.OpGetSteps, e.ToString());
                    return;
                }

                int result = 0;

                string message = "";
                if (retVals[0].Maintype() != (int)ParamType.SimpleType.error)
                {
                    result = Convert.ToInt32(retVals[0].Value());
                    message = String.Format("{0} success to {1} result = {2}", RoleFitbitUltra.OpGetSteps, fitbitPort.ToString(), result);
                }
                else
                {
                    message = String.Format("{0} failure to {1}", RoleFitbitUltra.OpGetSteps, fitbitPort.ToString());
                    return;
                }

                LogMessageToWindow(message);

                currentSteps = result;
            }
        }

        public void SendGetDeviceCurrentStepsGoalMethod(int counter)
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

                    retVals = fitbitPort.Invoke(RoleFitbitUltra.RoleName, RoleFitbitUltra.OpGetStepsGoal, parameters, ControlPort, fitbitPortCapability, ControlPortCapability);
                }
                catch (Exception e)
                {
                    logger.Log("Error while calling {0} request: {1}", RoleFitbitUltra.OpGetStepsGoal, e.ToString());
                    return;
                }

                int result = 0;

                string message = "";
                if (retVals[0].Maintype() != (int)ParamType.SimpleType.error)
                {
                    result = Convert.ToInt32(retVals[0].Value());
                    message = String.Format("{0} success to {1} result = {2}", RoleFitbitUltra.OpGetStepsGoal, fitbitPort.ToString(), result);
                }
                else
                {
                    message = String.Format("{0} failure to {1}", RoleFitbitUltra.OpGetStepsGoal, fitbitPort.ToString());
                    return;
                }

                LogMessageToWindow(message);

                if (currentSteps >= result)
                {
                    // enable TV
                    counter++;
                    if (SendGetSwitchStatusMethod(counter) == 0)
                    {
                        counter++;
                        SendTurnOnSwitchMethod(counter);
                    }

                    LogMessageToWindow("User has met their steps goal for today, TV has been enabled.");
                }
                else
                {
                    // disable TV
                    counter++;
                    if (SendGetSwitchStatusMethod(counter) != 0)
                    {
                        counter++;
                        SendTurnOffSwitchMethod(counter);
                    }

                    LogMessageToWindow("User has not met their steps goal for today, TV has been disabled.");
                }
            }
        }

        public int SendGetSwitchStatusMethod(int counter)
        {
            if (switchPort != null && switchPortCapability == null)
            {
                switchPortCapability = GetCapability(switchPort, Globals.UserSystem);
            }

            if (switchPort != null && switchPortCapability != null)
            {
                IList<View.VParamType> retVals = null;

                try
                {
                    IList<View.VParamType> parameters = new List<View.VParamType>();

                    retVals = switchPort.Invoke(RoleSwitchBinary.RoleName, RoleSwitchBinary.OpGetName, parameters, ControlPort, switchPortCapability, ControlPortCapability);
                }
                catch (Exception e)
                {
                    logger.Log("Error while calling {0} request: {1}", RoleSwitchBinary.OpGetName, e.ToString());
                }

                int result = 0;

                string message = "";
                if (retVals[0].Maintype() != (int)ParamType.SimpleType.error)
                {
                    result = Convert.ToInt32(retVals[0].Value());
                    message = String.Format("{0} success to {1} result = {2}", RoleSwitchBinary.OpGetName, switchPort.ToString(), result);
                }
                else
                {
                    message = String.Format("{0} failure to {1}", RoleSwitchBinary.OpGetName, switchPort.ToString());
                }

                LogMessageToWindow(message);

                return result;
            }

            return -1;
        }

        public void SendTurnOnSwitchMethod(int counter)
        {
            if (switchPort != null && switchPortCapability == null)
            {
                switchPortCapability = GetCapability(switchPort, Globals.UserSystem);
            }

            if (switchPort != null && switchPortCapability != null)
            {
                IList<View.VParamType> retVals = null;

                try
                {
                    IList<View.VParamType> parameters = new List<View.VParamType>();
                    parameters.Add(new ParamType(ParamType.SimpleType.integer, "8", (byte)255, "level"));

                    retVals = switchPort.Invoke(RoleSwitchBinary.RoleName, RoleSwitchBinary.OpSetName, parameters, ControlPort, switchPortCapability, ControlPortCapability);
                }
                catch (Exception e)
                {
                    logger.Log("Error while calling {0} request: {1}", RoleSwitchBinary.OpSetName, e.ToString());
                }

                int result = 0;

                string message = "";
                if (retVals[0].Maintype() != (int)ParamType.SimpleType.error)
                {
                    result = Convert.ToInt32(retVals[0].Value());
                    message = String.Format("{0} success to {1} result = {2}", RoleSwitchBinary.OpSetName, switchPort.ToString(), result);
                }
                else
                {
                    message = String.Format("{0} failure to {1}", RoleSwitchBinary.OpSetName, switchPort.ToString());
                }

                LogMessageToWindow(message);
            }
        }

        public void SendTurnOffSwitchMethod(int counter)
        {
            if (switchPort != null && switchPortCapability == null)
            {
                switchPortCapability = GetCapability(switchPort, Globals.UserSystem);
            }

            if (switchPort != null && switchPortCapability != null)
            {
                IList<View.VParamType> retVals = null;

                try
                {
                    IList<View.VParamType> parameters = new List<View.VParamType>();
                    parameters.Add(new ParamType(ParamType.SimpleType.integer, "8", (byte)0, "level"));

                    retVals = switchPort.Invoke(RoleSwitchBinary.RoleName, RoleSwitchBinary.OpSetName, parameters, ControlPort, switchPortCapability, ControlPortCapability);
                }
                catch (Exception e)
                {
                    logger.Log("Error while calling {0} request: {1}", RoleSwitchBinary.OpSetName, e.ToString());
                }

                int result = 0;

                string message = "";
                if (retVals[0].Maintype() != (int)ParamType.SimpleType.error)
                {
                    result = Convert.ToInt32(retVals[0].Value());
                    message = String.Format("{0} success to {1} result = {2}", RoleSwitchBinary.OpSetName, switchPort.ToString(), result);
                }
                else
                {
                    message = String.Format("{0} failure to {1}", RoleSwitchBinary.OpSetName, switchPort.ToString());
                }

                LogMessageToWindow(message);
            }
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

                if (Role.ContainsRole(port, RoleSwitchBinary.RoleName))
                {
                    switchPort = port;
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

                if (port.Equals(switchPort))
                {
                    switchPort = null;
                    logger.Log("{0} removed switchbinary port {0}", this.ToString(), port.ToString());

                    if (logWindow != null)
                        logWindow.Invoke(new Action(delegate() { logWindow.ConsoleLog("removed port " + port.ToString()); }));
                }
            }
        }

        #endregion
    }
}