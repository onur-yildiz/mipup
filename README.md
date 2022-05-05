# Mipup 

Console Application Tool for cutting audio from a Youtube video and uploading it to Myinstants automatically.

	-n, --name     Required. The name you want the audio to be saved as.

	-u, --url      Required. URL of the youtube video for audio to cut from. (https://www.youtube.com/watch?v=7tJi2tyGmEI)

	-s, --start    Required. Start position of the audio. (hh:mm:ss)

	-t, --time     Required. Length of the audio. (Seconds)

	--help         Display this help screen.

	--version      Display version information.

## Example

	./mipup.exe -n myaudio -u https://www.youtube.com/watch?v=7tJi2tyGmEI -s 1:58 -t 12

**NOT SECURE!!**\
Stores your **Myinstants** credentials as plain text **ONLY** if you **AGREE**. Otherwise you have to enter your credentials every time.\
No **Youtube** credentials needed.

**TESTED ON 05.05.2022** If Myinstants website gets updated puppet may not work correctly. Please open an issue or feel free to PR a fix if that's the case.

---

*Copyright (C) 2022 Mipup 1.0.0*