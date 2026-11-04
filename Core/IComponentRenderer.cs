using System;
using Rocket.Core.DOM;

namespace Rocket.Core
{
    /// <summary>
    /// Renderer responsible for drawing virtual DOM nodes.
    /// </summary>
    public interface IComponentRenderer
    {
        void Draw(Node rootNode);
    }
}