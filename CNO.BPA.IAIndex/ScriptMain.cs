using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Custom.InputAccel.UimScript
{
    using Emc.InputAccel.UimScript;

    public class ScriptMain : UimScriptMainCore
    {
        public int lastDocNodeId { get; set; }

        public ScriptMain()
            : base()
        {

        }

        public override void ScriptLoad()
        {
            base.ScriptLoad();
            Share.setTree(new LinkedList<DocumentNode>());
            Share.setOperatorID(IALoginUserName);
            
        }

        public override void ScriptUnload()
        {
            base.ScriptUnload();

        }
        
        protected override void NodeAdded(IUimNodeData node, IUimNodeData parent, GlobalEventReason reason)
        {

            base.NodeAdded(node, parent, reason);

            if (node.Level == 0)
            { // page added
                //MessageBox.Show("Page added");
                LinkedListNode<DocumentNode> current = Share.getTree().First;
                while (current != null)
                {
                    if (current.Value.getNodeId() == parent.NodeId)
                    {
                        current.Value.setPageCount(current.Value.getPageCount() + 1);
                        break;
                    }
                    current = current.Next;
                }

            }
            else if (node.Level == 1)
            { // doc added
              //MessageBox.Show("Document Added. Reason: " + reason.ToString() + " NodeId: " + node.NodeId);
                if (reason == 0)
                {
                    MessageBox.Show("Illegal Operation: '\"New Empty Document\" is an invalid operation. Please use \"New Document\" operation instead. Batch corrupted!");
                }

                Share.getNewNode();     //21.4 Captiva Upgrade change- adding new node split into tree
                Share.setNewNode(new DocumentNode()); //21.4 Captiva Upgrade
                Share.getNewNode().setNodeId(node.NodeId);


                int lastSelectedNode = Share.getLastSelectedNode().getNodeId();
                LinkedListNode<DocumentNode> current = Share.getTree().First;
                while (current != null)
                {
                    if (current.Value.getNodeId() == lastSelectedNode)
                    {
                        break;
                    }
                    current = current.Next;
                }
                //LinkedListNode<int> current = Share.getTree().Find(Convert.ToInt32(Share.getProperties()["NODE_ID"]));
              
                /* MOVING THE CODE ABOVE TOLINE 60 27TH JULY 22
                Share.getNewNode();     //21.4 Captiva Upgrade change- adding new node split into tree
                Share.setNewNode(new DocumentNode()); //21.4 Captiva Upgrade
                Share.getNewNode().setNodeId(node.NodeId);

                */
                if (node.ChildrenCount == 0)
                {
                    Share.getNewNode().setEmpty(true);
                    Share.getNewNode().setPageCount(0);
                    
                    //if (current.Previous != null && current.Previous.Value != null && current.Previous.Value.getNodeId() == 0) //Blank doc added
                    //{
                        Share.getTree().AddBefore(current, Share.getNewNode());
                    //}
                    //else //Doc splitted and resulted in a blank doc
                    //{
                    //    Share.getTree().AddAfter(current, Share.getNewNode());
                    //}
                }
                else //if (current.Value.getPageCount() == node.ChildrenCount) // Doc splitted and blank doc
                {
                    int originalNodePageCount = current.Value.getPageCount();
                    int newNodePageCount = node.ChildrenCount;

                    //Set new node page count
                    Share.getNewNode().setPageCount(newNodePageCount);
                    //Set original node page count
                    current.Value.setPageCount(originalNodePageCount-newNodePageCount);
                    if (originalNodePageCount - newNodePageCount == 0)
                    {
                        current.Value.setEmpty(true);
                    }

                    Share.getTree().AddAfter(current, Share.getNewNode());
                    //if (current.Value.
                   //7july toset last node
                    //Share.setLastSelectedNode(Share.getNewNode());
                }

                if (reason == GlobalEventReason.NodeSplit)
                {
                    

                   /* if (Share.getProperties()["LASTDOC"].ToUpper() == "Y")
                    {
                        Share.setSplittingLastDoc(true);
                    }*/
                }


                //Set NODE ID of new node.

                //Share.getProperties()["NODE_ID"] = Convert.ToString(node.NodeId);


                //Share.setLastDocNodeId(node.NodeId);
            }


        }

        protected override void NodeMoved(IUimNodeData node, IUimNodeData newParent, int newIndex, IUimNodeData oldParent, int oldIndex, GlobalEventReason reason)
        {
            base.NodeMoved(node, newParent, newIndex, oldParent, oldIndex, reason);

            if (node.Level == 0)
            {
                if (oldParent.NodeId == newParent.NodeId) //within doc
                {

                }
                else //page moved to different doc
                {
                    LinkedList<DocumentNode> tree = Share.getTree();

                    //find oldParent & newParent in the tree
                    LinkedListNode<DocumentNode> current = Share.getTree().First;
                    LinkedListNode<DocumentNode> oldNode = null;
                    LinkedListNode<DocumentNode> newNode = null;
                    int count = 0;
                    while (current != null)
                    {
                        if (current.Value.getNodeId() == oldParent.NodeId)
                        {
                            oldNode = current;
                            count++;
                        }
                        if (current.Value.getNodeId() == newParent.NodeId)
                        {
                            newNode = current;
                            count++;
                        }
                        if (count > 1) {
                            break;
                        }
                        current = current.Next;
                    }

                    try
                    {
                        //set child count
                        newNode.Value.setPageCount(newParent.ChildrenCount);
                        //oldNode.Value.setPageCount(oldParent.ChildrenCount-1);
                        oldNode.Value.setPageCount(oldParent.ChildrenCount);//21.4: oldparent count is already reduced to 1, no need to reduce again
                        if (newParent.ChildrenCount == 0)
                        {
                            newNode.Value.setEmpty(true);
                        }
                        else
                        {
                            newNode.Value.setEmpty(false);
                        }
                        //if (oldParent.ChildrenCount - 1 == 0)////21.4: oldparent count is already reduced to 1, no need to reduce again
                        if (oldParent.ChildrenCount == 0)
                        {
                            oldNode.Value.setEmpty(true);
                        }
                        else
                        {
                            oldNode.Value.setEmpty(false);
                        }
                    }
                    catch (Exception ex)
                    {

                    }

                }
                
            }
            else if (node.Level == 1)// node.level == 1
            {
                int nodeIdToFind = Convert.ToInt32(node.NodeId); //The node that is being moved.
                LinkedListNode<DocumentNode> currentNode = Share.getTree().First;
                LinkedListNode<DocumentNode> locationToAdd = null;
                LinkedListNode<DocumentNode> nodeBeingMoved = null;
                int count = 0;
                while (currentNode != null)
                {
                    if (currentNode.Value.getNodeId() == nodeIdToFind)
                    {
                        nodeBeingMoved = currentNode;
                    }
                    if (newIndex == count)
                    {
                        locationToAdd = currentNode;
                    }
                    currentNode = currentNode.Next;
                    count++;
                }


                Share.getTree().Remove(nodeBeingMoved);
                Share.getTree().AddBefore(locationToAdd, nodeBeingMoved);
                

                //LinkedListNode<int> current = Share.getTree().Find(Convert.ToInt32(Share.getProperties()["NODE_ID"]));
                DocumentNode newDoc = new DocumentNode(node.NodeId);
                if (node.ChildrenCount == 0)
                {
                    newDoc.setEmpty(true);
                }

                /*if (Share.getLastDocNodeId() == node.NodeId)
                {
                    Share.setLastDocNodeId(Share.getTree().Last.Value.getNodeId());
                }*/
                //MessageBox.Show("Document Moved. Reason: " + reason.ToString() + " NodeId: " + node.NodeId);
            }


        }

        protected override void NodeDeleted(IUimNodeData node, IUimNodeData oldParent, GlobalEventReason reason)
        {
            base.NodeDeleted(node, oldParent, reason);

            if (node.Level == 0)
            {
                LinkedListNode<DocumentNode> current = Share.getTree().First;
                while (current != null)
                {
                    if (current.Value.getNodeId() == oldParent.NodeId)
                    {
                        current.Value.setPageCount(current.Value.getPageCount() - 1);
                        break;
                    }
                    current = current.Next;
                }

            }
            else if (node.Level == 1)
            {
                //MessageBox.Show("Document Deleted. Reason: " + reason.ToString() + " NodeId: " + node.NodeId);

                LinkedListNode<DocumentNode> current = Share.getTree().First;
                while (current != null)
                {
                    if (current.Value.getNodeId() == node.NodeId)
                    {
                        break;
                    }
                    current = current.Next;
                }

                if (current != null)
                {
                    Share.getTree().Remove(current);
                }
                if (reason == GlobalEventReason.NodeMerged)
                {
                    if (current.Previous != null)
                    {
                        int previousNodePageCount = current.Previous.Value.getPageCount();
                        previousNodePageCount += node.ChildrenCount;
                        current.Previous.Value.setPageCount(previousNodePageCount);
                    }
                    //Share.getProperties()["NODE_ID"] = Convert.ToString(node.NodeId);
                    /*if (Share.getProperties()["LASTDOC"].ToUpper() == "Y")
                    {
                        Share.setLastDocNodeId(0);
                    }*/
                }

            }
        }


    }
}
