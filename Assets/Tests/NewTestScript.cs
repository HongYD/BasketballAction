using NUnit.Framework;
//Your using statements
using UnityEngine.InputSystem;

namespace Tests
{
    public class NewTestScript : InputTestFixture
    {
        [Test]
        public void NewTestScriptSimplePasses()
        {
            var gamepad = InputSystem.AddDevice<Gamepad>();
            Press(gamepad.buttonSouth);
        }
    }
}
