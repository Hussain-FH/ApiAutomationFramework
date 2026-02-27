using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiAutomationFramework.Models.Request.SOPConfig
{
    public class SOPConfigUpdateRequest
    {
       
            public int id { get; set; }
            public string sopConfigurationName { get; set; }
            public string fileRegex { get; set; }
            public List<string> countryLookupOrder { get; set; }
            public bool isAssociationRulesEnabled { get; set; }
            public string emailRecipients { get; set; }
            public bool isEnabled { get; set; }
            public int pluginClassId { get; set; }
            public bool isMappedProgramNameUsed { get; set; }
            public bool flagOrdersAsTest { get; set; }
            public bool isDuplicateEmbossingFileAllowed { get; set; }
            public bool isAllRecipientAddressesSaved { get; set; }
            public string fileEncodingCodeId { get; set; }
        


    }
}
