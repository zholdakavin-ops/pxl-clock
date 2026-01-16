#:package Pxl@0.0.34

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;
using Pxl.Ui.CSharp;
using static Pxl.Ui.CSharp.DrawingContext;






var scene = () =>
{



};

// Simulator
// await PXL.Simulate(scene);

// Gerät
await PXL.SendToDevice(scene, "192.168.178.110");





// await PXL.Simulate(scene);

await PXL.SendToDevice(scene, "192.168.178.110");