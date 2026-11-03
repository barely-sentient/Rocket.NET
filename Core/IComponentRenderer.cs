using System;
using Rocket.Core.DOM;

namespace Rocket.Core
{
    public interface IComponentRenderer
    {
        void Draw(Node rootNode);
    }
}