// Copyright 2022-2023 Niantic.

using Unity.Netcode.Components;

// TODO: Wrap with Niantic.Lightship.AR.Samples namespace
public class ClientNetworkTransformNew : NetworkTransform
{
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}
