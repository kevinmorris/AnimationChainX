# AnimationChainX

Implements sequential chaining of animations in iOS

## The Premise

iOS includes a mechanism for running multiple CAAnimations concurrently called CAAnimationGroup.  However, it does not include a similar ability to run multiple CAAnimations in series.  There are a variety of solutions to the problem. You chain together multiple instances of CAAnimationDelegate and listen for each animation to stop before launching the next one.  However this can lead to a kind of delegate pyramid of doom.  Another possibility is running multiple animations as part of a CAAnimationGroup but meticulously hardcoding a staggered beginTime or timeOffset on each animation in the group.  These solutions tend to be ugly or error prone.

## Installation and Usage

AnimationChainX features a single class: AnimationChain that allows you to chain together multiple animations potentially using different CALayers.  AnimationChainX is written in C# as part of the Xamarin platform.  You may compile this source into a dll and reference it in a Xamarin.iOS project OR you may follow these instructions to build the project into a native iOS library for any other type of native iOS codebase:

[https://developer.xamarin.com/guides/ios/advanced_topics/native_interop/](https://developer.xamarin.com/guides/ios/advanced_topics/native_interop/)

All of the following examples are written in C#.

### Launching a single CAAnimation

```C#
    var path = new UIBezierPath();
    path.MoveTo(start);
    path.AddQuadCurveToPoint(end, new CGPoint(end.X + ((start.X - end.X) / 2), 0));

    var pathAnimation = CAKeyFrameAnimation.FromKeyPath("position");
    pathAnimation.Path = path.CGPath;

    AnimationChain.Create(MyView.Layer, pathAnimation).Start();
```

This however is not doing anything particularly special.

### Chaining together multiple CAAnimations serially

```C#
    var path = new UIBezierPath();
    path.MoveTo(start);
    path.AddQuadCurveToPoint(end, new CGPoint(end.X + ((start.X - end.X) / 2), 0));

    var pathAnimation = CAKeyFrameAnimation.FromKeyPath("position");
    pathAnimation.Path = path.CGPath;

    var sizeAnimation = CABasicAnimation.FromKeyPath("transform");
    sizeAnimation.From = NSValue.FromCATransform3D(CATransform3D.MakeScale(0, 0, 1));
    sizeAnimation.To = NSValue.FromCATransform3D(CATransform3D.Identity);

    var rotateAnimation = CAKeyFrameAnimation.FromKeyPath("transform");
    rotateAnim.Values = new[] {
        NSValue.FromCATransform3D(CATransform3D.Identity),
        NSValue.FromCATransform3D(CATransform3D.MakeRotation(3.14f / 8, 0, 0, -1.0f)),
        NSValue.FromCATransform3D(CATransform3D.MakeRotation(-3.14f / 8, 0, 0, -1.0f)),
        NSValue.FromCATransform3D(CATransform3D.Identity),
    };

    AnimationChain.Create(MyView.Layer, pathAnimation).Chain((layer, animation) =>
    {
        return AnimationChain.Create(MyView.Layer, sizeAnimation);
    }).Chain((layer, animation) =>
    {
        return AnimationChain.Create(MyView.Layer, rotationAnimation);
    }).Start();
```

This code animates a view's layer by position, followed by its apparent size followed by a rotation transform.
