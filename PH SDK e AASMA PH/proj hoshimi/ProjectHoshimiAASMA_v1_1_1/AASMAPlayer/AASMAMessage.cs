using System;
using System.Collections.Generic;
using System.Text;

namespace AASMAHoshimi
{
    public class AASMAMessage
    {
        private string _sender = null;
        private string _receiver = null;
        private string _content = null;
        private Object _tag = null;

        public Object Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }

        public string Content
        {
            get { return _content; }
            set { _content = value;}
        }

        public string Receiver
        {
            get { return _receiver; }
            set { _receiver = value; }
        }

        public string Sender
        {
            get { return _sender; }
            set { _sender = value; }
        }

        public AASMAMessage(string sender, string content)
        {
            this._sender = sender;
            this._content = content;
        }

    }
}
