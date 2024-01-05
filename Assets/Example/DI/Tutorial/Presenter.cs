using VContainer;
using VContainer.Unity;

namespace PSB.DI.Tutorial
{
    public class Presenter : IStartable
    {
        readonly Service _helloWorldService;
        readonly HelloScreen _helloScreen;

        public Presenter(Service helloWorldService, HelloScreen helloScreen)
        {
            _helloWorldService = helloWorldService;
            _helloScreen = helloScreen;
        }

        void IStartable.Start()
        {
            _helloScreen._helloButton.onClick.AddListener(() => _helloWorldService.Hello());
        }
    }
}