using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Custom.InputAccel.UimScript
{
    using Emc.InputAccel.UimScript;
    using Emc.InputAccel.CaptureClient;

    static class Share
    {
        static bool dd_running = false;
        static LinkedList<DocumentNode> tree;
        static DocumentNode lastSelectedNode;
        static DocumentNode newNode;
        static string operatorID;
        //static int lastDocNodeId;  //removed comment on 7th july
        //static Dictionary<string, string> properties;
        //static bool splittingLastDoc = false;

        public static bool getDD_Running()
        {
            return dd_running;
        }

        public static void setDD_Running(bool dd)
        {
            dd_running = dd;
        }

        public static DocumentNode getNewNode()
        {
            return newNode;
        }

        public static void setNewNode(DocumentNode node)
        {
            newNode = node;
        }

        public static LinkedList<DocumentNode> getTree()
        {
            return tree;
        }

        public static void setTree(LinkedList<DocumentNode> list)
        {
            tree = list;
        }
        // remove comment on 7th july
        //public static int getLastDocNodeId()
        //{
        //    return lastDocNodeId;
        //}

        //public static void setLastDocNodeId(int nodeId)
        //{
        //    lastDocNodeId = nodeId;
        //}
        /*
        public static Dictionary<string, string> getProperties()
        {
            return properties;
        }

        public static void SetProperties(Dictionary<string, string> dict)
        {
            properties = dict;
        }
        */
        /*public static bool getSplittingLastDoc()
        {
            return splittingLastDoc;
        }

        public static void setSplittingLastDoc(bool split)
        {
            splittingLastDoc = split;
        }*/

        public static DocumentNode getLastSelectedNode()
        {
            return lastSelectedNode;
        }

        public static void setLastSelectedNode(DocumentNode nodeId)
        {
            lastSelectedNode = nodeId;
        }

        public static void setOperatorID(string _operatorID)
        {
            operatorID = _operatorID;
        }

        public static string getOperatorID()
        {
            return operatorID;
        }
    }
}

/*
 * using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Custom.InputAccel.UimScript
{
    using Emc.InputAccel.UimScript;
    using Emc.InputAccel.CaptureClient;
    
    static class Common
    {
        static int lastDocNodeId;

        public static int getLastDocNodeId()
        {
            return lastDocNodeId;
        }

        public static void setLastDocNodeId(int nodeId)
        {
            lastDocNodeId = nodeId;
        }

        /*public static prepareTask(IBatchNode node) {
            try
            {
            IIAValueProvider nodeValueProvider = taskInfo.TaskRootNode.Values(taskInfo.Application.CurrentTask.WorkflowStep);
            string nodePath = "$node=" + taskInfo.TaskRootNode.Id.ToString();
            string ddrunning = nodeValueProvider.GetString(nodePath + "/$instance=Standard_MDF/E_DDRUNNING", "");
            if (ddrunning == "T")
            {
            //set ddrunning to f in case task was previously closed improperly
            nodeValueProvider.SetString(nodePath + "/$instance=Standard_MDF/E_DDRUNNING", "F");
            }
            }
            catch (Exception ex)
            {
            MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}

 */
