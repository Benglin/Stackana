using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace Xleeque.Stackana.Logics
{
    abstract public class GameObject
    {
        private Canvas parent_canvas;
        private GameObject parent_object;
        private List<GameObject> children_list;

        public GameObject(GameObject parentObject, Canvas parentCanvas)
        {
            this.parent_object = parentObject;
            this.parent_canvas = parentCanvas;
        }

        /// <summary>
        /// This method must be implemented in derived classes in order to
        /// properly initialize the object with data members specific to 
        /// derived classes.
        /// </summary>
        abstract public void StartUp();

        /// <summary>
        /// This method must be implemented in derived classes in order to
        /// properly uninitialize the object with data members specific to 
        /// derived classes.
        /// </summary>
        abstract public void ShutDown();

        /// <summary>
        /// This method is called when the time is up for a frame update.
        /// </summary>
        /// <param name="deltaTime">Delta time in milliseconds since last 
        /// frame.</param>
        abstract public void UpdateFrame(double deltaTime);

        /// <summary>
        /// This method is overridden in the derived classes in order to 
        /// process keyboard input events.
        /// </summary>
        /// <param name="key">Indicates the key pressed/released.</param>
        /// <param name="keyDown">This parameter is set to true if key is
        /// pressed, or false otherwise.</param>
        abstract public void ProcessKeyInput(Key key, bool keyDown);

        public List<GameObject> Children
        {
            get
            {
                if (null == children_list)
                    children_list = new List<GameObject>();

                return children_list;
            }
        }

        public GameObject Parent
        {
            get { return parent_object; }
        }

        public Canvas ParentCanvas
        {
            get { return parent_canvas; }
        }
    }
}
