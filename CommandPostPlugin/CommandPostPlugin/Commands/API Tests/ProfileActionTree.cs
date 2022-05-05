
// TODO: this._applications is undefined.

namespace Loupedeck.CommandPostPlugin
{
    using System;

    using Fleck;

    class ProfileActionTree : PluginDynamicCommand
    {
        public ProfileActionTree() : base("Profile Action Tree", "Trigger a Profile Action", "API Tests") => this.MakeProfileAction("tree");

        //protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize) => this._applications.TryGetValue(actionParameter, out var application) ? application.ApplicationName : null;

        protected override PluginProfileActionData GetProfileActionData()
        {
            // create tree data

            var tree = new PluginProfileActionTree("Select Windows Settings Application");

            // describe levels

            tree.AddLevel("Category");
            tree.AddLevel("Application");

            // add data tree

            /*
            var categoryNames = this._applications.Values.Select(a => a.CategoryName).Distinct();

            foreach (var categoryName in categoryNames)
            {
                var node = tree.Root.AddNode(categoryName);

                var items = this._applications.Values.Where(a => a.CategoryName.EqualsNoCase(categoryName));

                foreach (var item in items)
                {
                    node.AddItem(item.ApplicationUri, item.ApplicationName, null);
                }
            }
            */

            // return tree data

            return tree;
        }

        protected override void RunCommand(String actionParameter)
        {
            //
            // This is what's triggered when you run the "Simple Command" (i.e. you press a button):
            //
            foreach (IWebSocketConnection socket in Loupedeck.CommandPostPlugin.CommandPostPlugin.allSockets)
            {
                socket.Send(actionParameter);
            }
        }
    }
}