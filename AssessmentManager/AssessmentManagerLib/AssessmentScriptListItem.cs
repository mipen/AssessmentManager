﻿using System;

namespace AssessmentManager
{
    [Serializable]
    public class AssessmentScriptListItem
    {
        private AssessmentScript script = null;
        private string name = "";
        public AssessmentScriptListItem(AssessmentScript script, string name)
        {
            this.script = script;
            this.name = name;
        }

        public AssessmentScript Script
        {
            get
            {
                return script;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
