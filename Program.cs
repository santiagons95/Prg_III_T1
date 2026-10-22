using DocuTrack.Controller;
using DocuTrack.Model;
using DocuTrack.View;

// Punto de entrada: solo ensambla las capas e invoca al Controller.
new DocuTrackController(new ArbolBinario(), new ConsolaVista()).Ejecutar();
