
namespace Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Role : View.VRole
    {
        private string name;
        private Dictionary<string, View.VOperation> operations = new Dictionary<string,View.VOperation>();

        public Role(string name)
        {
            this.name = name.ToLower();
        }

        public void AddOperation(View.VOperation operation)
        {
            lock (operations)
            {
                if (operations.ContainsKey(operation.Name()))
                    throw new Exception("operation name " + operation.Name() + " already exists!");

                operations.Add(operation.Name().ToLower(), operation);

            }
        }

        public View.VOperation GetOperation(string opName)
        {
            lock (operations)
            {
                if (operations.ContainsKey(opName)) 
                    return operations[opName.ToLower()];
            }

            return null;
        }

        //public View.VBaseType MakeReturnValue(string opName, int index, object value)
        //{
        //    View.VOperation operation = GetOperation(opName);

        //    if (operation == null) 
        //            throw new Exception("Illegal operation name " + opName);

        //    if (index >= operation.ReturnValues().Count)
        //            throw new Exception("Illegal index " + index + " for retvalue of " + opName);

        //     View.VBaseType baseType = operation.ReturnValues()[index];

        //     return new BaseType( (BaseType.SimpleType) baseType.Type(), baseType.Subtype(), value);
            
        //}

        //public View.VBaseType MakeParameter(string opName, int index, object value)
        //{
        //    View.VOperation operation = GetOperation(opName);

        //    if (operation == null) 
        //            throw new Exception("Illegal operation name " + opName);

        //    if (index >= operation.Parameters().Count)
        //            throw new Exception("Illegal index " + index + " for parameter of " + opName);

        //     View.VBaseType baseType = operation.Parameters()[index];

        //     return new BaseType( (BaseType.SimpleType) baseType.Type(), baseType.Subtype(), value);
        //}


        public override IList<View.VOperation> GetOperations()
        {
            IList<View.VOperation> ret;
            lock (operations)
            {
                ret = operations.Values.ToList();
            }
            return ret;
        }

        public static bool ContainsRole(View.VPort port, string roleName)
        {
            foreach (View.VRole role in port.GetInfo().GetRoles())
                if (role.Name().Equals(roleName, StringComparison.CurrentCultureIgnoreCase))
                    return true;
            return false;
        }

        public override string Name()
        {
            return this.name;
        }

        public override string ToString()
        {
            return name;
        }
    }

    public class RoleIRLearner : Role
    {
        public const string RoleName = "IRLearner";
        public const string OpLearnName = "Learn";
        public const string ReturnTypeName = "irlearn";
        // exporting string Learn(void)
        public RoleIRLearner() : base(RoleName)
        {            
            List<View.VParamType> retTypes = new List<View.VParamType>();
            retTypes.Add(new ParamType(ParamType.SimpleType.text, "", "", ReturnTypeName));
            AddOperation(new Operation(OpLearnName, null, retTypes));
        }
    }

    public class RoleIRSender : Role
    {
        public const string RoleName = "IRSender";
        public const string OpSendName = "Send";
        public const string ArgTypeName = "irsend";
        // exporting void Send(string)
        public RoleIRSender() : base(RoleName)
        {
            List<View.VParamType> argTypes = new List<View.VParamType>();
            argTypes.Add(new ParamType(ParamType.SimpleType.text, "", "", ArgTypeName));

            List<View.VParamType> retTypes = new List<View.VParamType>();
            retTypes.Add(new ParamType(ParamType.SimpleType.binary, "", null, "status"));

            AddOperation(new Operation(OpSendName, argTypes, retTypes));
        }
    }

    public class RoleDummy : Role
    {
        public const string RoleName = "dummy";
        public const string OpEchoName = "echo";
        public const string OpEchoSubName = "echosub";

        public RoleDummy()
            : base(RoleName)
        {
            {
                List<View.VParamType> args = new List<View.VParamType>();
                args.Add(new ParamType(ParamType.SimpleType.integer, "", 0, "echonum"));

                List<View.VParamType> retVals = new List<View.VParamType>();
                retVals.Add(new ParamType(ParamType.SimpleType.integer, "", 0, "responsenum"));

                AddOperation(new Operation(OpEchoName, args, retVals));
            }

            {
                List<View.VParamType> retVals = new List<View.VParamType>();
                retVals.Add(new ParamType(ParamType.SimpleType.integer, "", 0, "responsenum"));

                AddOperation(new Operation(OpEchoSubName, null, retVals, true));
            }
        }
    }

    public class RoleImgRec : Role
    {
        public const string RoleName = "image-recognize";
        public const string OpRecognizeName = "Recognize";

        public RoleImgRec()
            : base(RoleName)
        {
            IList<View.VParamType> args = new List<View.VParamType>();
            args.Add(new ParamType(ParamType.SimpleType.image, System.Net.Mime.MediaTypeNames.Image.Jpeg, null, "image"));

            IList<View.VParamType> retVals = new List<View.VParamType>();
            retVals.Add(new ParamType(ParamType.SimpleType.integer, "", null, "id"));

            AddOperation(new Operation(OpRecognizeName, args, retVals));
        }
    }

    public class RoleDms : Role
    {
        public const string RoleName = "dms";
        public const string OpListMediaName = "listmedia";

        public RoleDms()
            : base(RoleName)
        {
            AddOperation(new Operation(RoleDms.OpListMediaName, null, null));
        }
    }

    public class RoleDmr : Role
    {
        public const string RoleName = "dmr";
        public const string OpPlayName = "play";
        public const string OpPlayAtName = "playat";
        public const string OpStopName = "stop";
        public const string OpStatusRequestName = "statusrequest";

        public RoleDmr()
            : base(RoleName)
        {
            {
                List<View.VParamType> playParameters = new List<View.VParamType>();
                playParameters.Add(new ParamType(ParamType.SimpleType.text, "", null, "uri"));
                AddOperation(new Operation(OpPlayName, playParameters, null));  //play(uri)
            }
            {
                List<View.VParamType> playAtParameters = new List<View.VParamType>();
                playAtParameters.Add(new ParamType(ParamType.SimpleType.text, "", null, "url"));
                playAtParameters.Add(new ParamType(ParamType.SimpleType.text, "", null, "time"));
                AddOperation(new Operation(OpPlayAtName, playAtParameters, new List<View.VParamType>())); //play(uri,time)
            }

            {
                AddOperation(new Operation(OpStopName, null, null)); //stop()
            }

            {
                List<View.VParamType> statusRets = new List<View.VParamType>();
                statusRets.Add(new ParamType(ParamType.SimpleType.text, "", null, "uri"));
                statusRets.Add(new ParamType(ParamType.SimpleType.text, "", null, "time"));
                AddOperation(new Operation(OpStatusRequestName, new List<View.VParamType>(), statusRets)); //(uri,time) <- statusrequest
            }
        }
    }

    public class RoleSensor : Role
    {
        public const string RoleName = "sensor";
        public const string OpGetName = "get";

        public RoleSensor()
            : base(RoleName)
        {
            {
                List<View.VParamType> getReturnVals = new List<View.VParamType>();
                getReturnVals.Add(new ParamType(ParamType.SimpleType.integer, "8", null, "value"));
                AddOperation(new Operation(OpGetName, null, getReturnVals, true));
            }
        }
    }

    public class RoleSwitchMultiLevel : Role
    {
        public const string RoleName = "switchmultilevel";
        public const string OpSetName = "set";
        public const string OpGetName = "get";

        public RoleSwitchMultiLevel()
            : base(RoleName)
        {
            {
                List<View.VParamType> setParameters = new List<View.VParamType>();
                setParameters.Add(new ParamType(ParamType.SimpleType.integer, "8", null, "level"));
                AddOperation(new Operation(OpSetName, setParameters, null));
            }

            {
                List<View.VParamType> getReturnVals = new List<View.VParamType>();
                getReturnVals.Add(new ParamType(ParamType.SimpleType.integer, "8", null, "level"));
                AddOperation(new Operation(OpGetName, null, getReturnVals, true));
            }
        }
    }

    public class RoleSwitchBinary : Role
    {
        public const string RoleName = "switchbinary";
        public const string OpSetName = "set";
        public const string OpGetName = "get";

        public RoleSwitchBinary()
            : base(RoleName)
        {
            {
                List<View.VParamType> setParameters = new List<View.VParamType>();
                setParameters.Add(new ParamType(ParamType.SimpleType.integer, "8", null, "level"));
                AddOperation(new Operation(OpSetName, setParameters, null));
            }

            {
                List<View.VParamType> getReturnVals = new List<View.VParamType>();
                getReturnVals.Add(new ParamType(ParamType.SimpleType.integer, "8", null, "level"));
                AddOperation(new Operation(OpGetName, null, getReturnVals, true));
            }
        }
    }

    public class RoleCamera : Role
    {
        public const string RoleName = "camera";
        public const string OpUpName = "up";
        public const string OpDownName = "down";
        public const string OpLeftName = "left";
        public const string OpRightName = "right";
        public const string OpZoomInName = "zoomin";
        public const string OpZommOutName = "zoomout";
        public const string OpGetImageName = "getimage";
        public const string OpGetVideo = "getvideo";

        public RoleCamera()
            : base(RoleName)
        {

            AddOperation(new Operation(RoleCamera.OpUpName, null, null));
            AddOperation(new Operation(RoleCamera.OpDownName, null, null));
            AddOperation(new Operation(RoleCamera.OpLeftName, null, null));
            AddOperation(new Operation(RoleCamera.OpRightName, null, null));
            AddOperation(new Operation(RoleCamera.OpZoomInName, null, null));
            AddOperation(new Operation(RoleCamera.OpZommOutName, null, null));

            {
                List<View.VParamType> retVals = new List<View.VParamType>();
                retVals.Add(new ParamType(ParamType.SimpleType.image, System.Net.Mime.MediaTypeNames.Image.Jpeg, null, "image"));

                AddOperation(new Operation(RoleCamera.OpGetImageName, null, retVals));
            }

            {
                List<View.VParamType> retVals = new List<View.VParamType>();
                retVals.Add(new ParamType(ParamType.SimpleType.image, System.Net.Mime.MediaTypeNames.Image.Jpeg, null, "image"));

                AddOperation(new Operation(RoleCamera.OpGetVideo, null, retVals, true));
            }

        }
    }

    public class RoleNotifications : Role
    {
        public const string RoleName = "notifications";
        public const string OpEmailName = "email";
        public const string OpSmsName = "sms";

        public RoleNotifications()
            : base(RoleName)
        {

            {
                List<View.VParamType> paramList = new List<View.VParamType>();
                paramList.Add(new ParamType(ParamType.SimpleType.text, "string", null, "subject"));
                paramList.Add(new ParamType(ParamType.SimpleType.text, "string", null, "message"));

                //more parameters may exist for attachments
                //attachments are of type ParamType.SimpleType.bytearray

                List<View.VParamType> retList = new List<View.VParamType>();
                paramList.Add(new ParamType(ParamType.SimpleType.binary, "", null, "result"));

                AddOperation(new Operation(RoleNotifications.OpEmailName, paramList, retList));
            }

            {
                List<View.VParamType> paramList = new List<View.VParamType>();
                paramList.Add(new ParamType(ParamType.SimpleType.text, "string", null, "subject"));
                paramList.Add(new ParamType(ParamType.SimpleType.text, "string", null, "message"));

                List<View.VParamType> retList = new List<View.VParamType>();
                paramList.Add(new ParamType(ParamType.SimpleType.binary, "", null, "result"));

                AddOperation(new Operation(RoleNotifications.OpSmsName, paramList, retList));
            }

        }
    }

    public class RoleFitbitUltra : Role
    {
        public const string RoleName = "fitbitUltra";

        public const string OpGetActiveScoreName = "getActiveScore";
        public const string OpGetCaloriesOutName = "getCaloriesOut";
        public const string OpGetDistanceName = "getDistance";
        public const string OpGetStepsName = "getSteps";

        public const string OpGetTotalMinutesAsleep = "getTotalMinutesAsleep";
        public const string OpGetTotalSleepRecords = "getTotalSleepRecords";
        public const string OpGetTotalTimeInBed = "getTotalTimeInBed";

        public const string OpGetDevicePresence = "getDevicePresence";

        public RoleFitbitUltra()
            : base(RoleName)
        {

            {
                List<View.VParamType> paramList = new List<View.VParamType>();
                paramList.Add(new ParamType(ParamType.SimpleType.text, "string", null, "consumerKey"));
                paramList.Add(new ParamType(ParamType.SimpleType.text, "string", null, "consumerSecret"));
                paramList.Add(new ParamType(ParamType.SimpleType.text, "string", null, "accessToken"));
                paramList.Add(new ParamType(ParamType.SimpleType.text, "string", null, "tokenSecret"));

                paramList.Add(new ParamType(ParamType.SimpleType.binary, "DateTime", null, "activityDate"));

                List<View.VParamType> retList = new List<View.VParamType>();
                paramList.Add(new ParamType(ParamType.SimpleType.integer, "", null, "result"));

                AddOperation(new Operation(RoleFitbitUltra.OpGetActiveScoreName, paramList, retList));
            }

            {
                List<View.VParamType> paramList = new List<View.VParamType>();
                paramList.Add(new ParamType(ParamType.SimpleType.text, "string", null, "consumerKey"));
                paramList.Add(new ParamType(ParamType.SimpleType.text, "string", null, "consumerSecret"));
                paramList.Add(new ParamType(ParamType.SimpleType.text, "string", null, "accessToken"));
                paramList.Add(new ParamType(ParamType.SimpleType.text, "string", null, "tokenSecret"));

                paramList.Add(new ParamType(ParamType.SimpleType.binary, "DateTime", null, "activityDate"));

                List<View.VParamType> retList = new List<View.VParamType>();
                paramList.Add(new ParamType(ParamType.SimpleType.integer, "", null, "result"));

                AddOperation(new Operation(RoleFitbitUltra.OpGetCaloriesOutName, paramList, retList));
            }

            {
                List<View.VParamType> paramList = new List<View.VParamType>();
                paramList.Add(new ParamType(ParamType.SimpleType.text, "string", null, "consumerKey"));
                paramList.Add(new ParamType(ParamType.SimpleType.text, "string", null, "consumerSecret"));
                paramList.Add(new ParamType(ParamType.SimpleType.text, "string", null, "accessToken"));
                paramList.Add(new ParamType(ParamType.SimpleType.text, "string", null, "tokenSecret"));

                paramList.Add(new ParamType(ParamType.SimpleType.binary, "DateTime", null, "activityDate"));

                List<View.VParamType> retList = new List<View.VParamType>();
                paramList.Add(new ParamType(ParamType.SimpleType.binary, "", null, "result"));

                AddOperation(new Operation(RoleFitbitUltra.OpGetDistanceName, paramList, retList));
            }

            {
                List<View.VParamType> paramList = new List<View.VParamType>();
                paramList.Add(new ParamType(ParamType.SimpleType.text, "string", null, "consumerKey"));
                paramList.Add(new ParamType(ParamType.SimpleType.text, "string", null, "consumerSecret"));
                paramList.Add(new ParamType(ParamType.SimpleType.text, "string", null, "accessToken"));
                paramList.Add(new ParamType(ParamType.SimpleType.text, "string", null, "tokenSecret"));

                paramList.Add(new ParamType(ParamType.SimpleType.binary, "DateTime", null, "activityDate"));

                List<View.VParamType> retList = new List<View.VParamType>();
                paramList.Add(new ParamType(ParamType.SimpleType.integer, "", null, "result"));

                AddOperation(new Operation(RoleFitbitUltra.OpGetStepsName, paramList, retList));
            }

            {
                List<View.VParamType> paramList = new List<View.VParamType>();
                paramList.Add(new ParamType(ParamType.SimpleType.text, "string", null, "consumerKey"));
                paramList.Add(new ParamType(ParamType.SimpleType.text, "string", null, "consumerSecret"));
                paramList.Add(new ParamType(ParamType.SimpleType.text, "string", null, "accessToken"));
                paramList.Add(new ParamType(ParamType.SimpleType.text, "string", null, "tokenSecret"));

                paramList.Add(new ParamType(ParamType.SimpleType.binary, "DateTime", null, "activityDate"));

                List<View.VParamType> retList = new List<View.VParamType>();
                paramList.Add(new ParamType(ParamType.SimpleType.integer, "", null, "result"));

                AddOperation(new Operation(RoleFitbitUltra.OpGetTotalMinutesAsleep, paramList, retList));
            }

            {
                List<View.VParamType> paramList = new List<View.VParamType>();
                paramList.Add(new ParamType(ParamType.SimpleType.text, "string", null, "consumerKey"));
                paramList.Add(new ParamType(ParamType.SimpleType.text, "string", null, "consumerSecret"));
                paramList.Add(new ParamType(ParamType.SimpleType.text, "string", null, "accessToken"));
                paramList.Add(new ParamType(ParamType.SimpleType.text, "string", null, "tokenSecret"));

                paramList.Add(new ParamType(ParamType.SimpleType.binary, "DateTime", null, "activityDate"));

                List<View.VParamType> retList = new List<View.VParamType>();
                paramList.Add(new ParamType(ParamType.SimpleType.integer, "", null, "result"));

                AddOperation(new Operation(RoleFitbitUltra.OpGetTotalSleepRecords, paramList, retList));
            }

            {
                List<View.VParamType> paramList = new List<View.VParamType>();
                paramList.Add(new ParamType(ParamType.SimpleType.text, "string", null, "consumerKey"));
                paramList.Add(new ParamType(ParamType.SimpleType.text, "string", null, "consumerSecret"));
                paramList.Add(new ParamType(ParamType.SimpleType.text, "string", null, "accessToken"));
                paramList.Add(new ParamType(ParamType.SimpleType.text, "string", null, "tokenSecret"));

                paramList.Add(new ParamType(ParamType.SimpleType.binary, "DateTime", null, "activityDate"));

                List<View.VParamType> retList = new List<View.VParamType>();
                paramList.Add(new ParamType(ParamType.SimpleType.integer, "", null, "result"));

                AddOperation(new Operation(RoleFitbitUltra.OpGetTotalTimeInBed, paramList, retList));
            }

            {
                List<View.VParamType> paramList = new List<View.VParamType>();
                paramList.Add(new ParamType(ParamType.SimpleType.text, "string", null, "consumerKey"));
                paramList.Add(new ParamType(ParamType.SimpleType.text, "string", null, "consumerSecret"));
                paramList.Add(new ParamType(ParamType.SimpleType.text, "string", null, "accessToken"));
                paramList.Add(new ParamType(ParamType.SimpleType.text, "string", null, "tokenSecret"));

                List<View.VParamType> retList = new List<View.VParamType>();
                paramList.Add(new ParamType(ParamType.SimpleType.binary, "", null, "result"));

                AddOperation(new Operation(RoleFitbitUltra.OpGetDevicePresence, paramList, retList));
            }

        }
    }
}