using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Custom.InputAccel.UimScript
{
    class DocumentNode
    {
        int nodeId;
        bool empty;
        int pageCount;
        private Dictionary<string, string> values;


        public int getPageCount()
        {
            return pageCount;
        }

        public void setPageCount(int newCount)
        {
            pageCount = newCount;
        }

        public DocumentNode()
        {
            nodeId = 0;
            empty = false;
        }

        public DocumentNode(int id)
        {
            nodeId = id;
            empty = false;
        }

        public Dictionary<string, string> getValues() {
            return values;
        }

        public void setValues(Dictionary<string, string> dictionaryValues)
        {
            values = dictionaryValues;
        }

        public bool isEmpty() {
            return empty;
        }

        public void setEmpty(bool _empty)
        {
            empty = _empty;
        }

        public int getNodeId()
        {
            return nodeId;
        }

        public void setNodeId(int id)
        {
            nodeId = id;
        }
    }
}
