# Assisted-Physics-XR-Rig-Hybrid
This project solves a problem with Valem's Physics XR Rig, in that non physics-driven objects do not influence the XR Rig. This is apparent when the player stands on a moving platform and is not moved (the moving platform using transform.position = ...).

Built on top of Valem's 4-part Tutorial: [https://youtu.be/gk0EBIe6ZN8?si=_o5zsPqcQE3CDGwQ](https://youtu.be/gk0EBIe6ZN8?si=_o5zsPqcQE3CDGwQ)

Using Spline-Trains with Configurable Joint Couplers by Paridot: [https://youtu.be/fNIrXVBuMqI?si=UQxnuhFMkX0ebK1R](https://youtube.com/playlist?list=PL_Ctd1Lny7CTBWEMjnUOhjLSXBzpvYfNO&si=5g9ybUAr09VdCqkF)

## Setup: 
  Simply import this into Unity and select your compatible headset in the Project Settings for XR and play the SampleScene.
  

## This features:
  - The preserving of position and rotation relative to the moving platform
  - Mobility on the moving platform

## Limitations: 
  - exceeding very fast speeds can cause the player to be unable to stay on the platform

Unity 6000.3.9f1
Unity XR Interaction Toolkit
