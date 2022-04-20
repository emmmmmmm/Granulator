# Granulator

A simple granular synthesiser built in C#.

### Summary.
Granulator is an experimental looking into granular sound synthesis in 3d space. It will spawn grains around it's current position, and optionally update the grain's position over their lifetime.
A custom spatialization plugin allows to set distance- and side attenuation curves, and calculates doppler effect pitch changes. -> Grains can have directionality!

Not using individual positions per grain would of course allow to create a much more efficient implementation by directly manipulating a single sample buffer, but having "3d grains" is worth the overhead for now.

### Applications.
Grain Synthesis presents a quick and simple way to create audio randomizations.
You can for example use a single ambience recording, and the Synth will pick random snippets of sound weave them together into an everchanging soundscape. (example included!)
Another application would be sound design for very dynamic engine sounds. All you need is a single audio-file that ranges from idle to full throttle, and control the position, and maybe the pitch of the grains according to the current engine-load. (There is an example for this in the project as well!)


### How to use it.
. set a sound file
. set the maximum number of simultaneous voices
. set grain position
. set grain length
. set time between grains
. set grain pitch
. have fun!

### Known issues:
. there is still some crackling happening when there are fast changes in pitch. 
