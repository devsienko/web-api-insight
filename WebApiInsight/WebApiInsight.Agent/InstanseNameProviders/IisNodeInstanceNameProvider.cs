using System;

namespace WebApiInsight.Agent
{
    public class IisNodeInstanceNameProvider : IInstanceNameProvider
    {
        //example of input: LM/Sites/Default Web Site/IisNodeName
        //example of result: _LM_W3SVC_1_ROOT_IisNodeName
        public string GetInstanseName(string nodeIdentifier)
        {
            throw new NotImplementedException();
        }
    }
}
