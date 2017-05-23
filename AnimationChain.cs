using System;
using CoreAnimation;

namespace AnimationChainX
{
    public class AnimationChain
    {
        CALayer _layer;
		CAAnimation _animation;

		private AnimationChain Previous { get; set; }

		AnimationChain _next;

		Func<CALayer, CAAnimation, AnimationChain> _callback;

		private AnimationChain(CALayer layer, CAAnimation animation)
		{
			_layer = layer;
			_animation = animation;
		}

		public AnimationChain Chain(Func<CALayer, CAAnimation, AnimationChain> callback)
		{
			_callback = callback;

			_next = callback(_layer, _animation);
			_next.Previous = this;
            _animation.Delegate = new AnimationChainDelegate(_next);

			return _next;
		}

		public static AnimationChain Create(CALayer layer, CAAnimation animation)
		{
			return new AnimationChain(layer, animation);
		}

		public void Start()
		{
			if (Previous != null)
			{
				Previous.Start();
			}
			else
			{
				Run();
			}
		}

		public void Run()
		{
			_layer.AddAnimation(_animation, null);
		}

        class AnimationChainDelegate : CAAnimationDelegate
        {
            readonly AnimationChain _next;

            public AnimationChainDelegate(AnimationChain next)
            {
                _next = next;
            }

            public override void AnimationStopped(CAAnimation anim, Boolean finished)
            {
                if (_next != null)
                {
                    _next.Run();
                }
            }
        }
    }
}
