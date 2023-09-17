
window.camera_interval = null;
// Basic settings for the video to get from Webcam
window.camera = (height, width, elementSelector = "video#barcode", bufferSelector = "canvas#buffer") => {
    const video = document.querySelector(elementSelector);
    const canvas = document.querySelector(bufferSelector);
    var tryTanslate = async () => {
        const context = canvas.getContext("2d");
        if (width && height) {
            canvas.width = width;
            canvas.height = height;
            context.drawImage(video, 0, 0, width, height);

            const data = canvas.toDataURL("image/png");

            var assName = "BardCoded.Pages.Nonmobile_Barcode";


            let result = await DotNet.invokeMethodAsync("BardCoded", "translate", data.split(",")[1], data.split(",")[0]);
            console.log("It . . .", result);
            if (result !== null) {
                clearInterval(window.camera_interval);
                console.log("done!");
            }

        } else {
            const context = canvas.getContext("2d");
            context.fillStyle = "#AAA";
            context.fillRect(0, 0, canvas.width, canvas.height);

            const data = canvas.toDataURL("image/png");
            photo.setAttribute("src", data);
        }
    }

    var constraints = {
        video: {
            mandatory: {
                minWidth: 1280,
                minHeight: 1280
            }
        }
    };

    function stop(e) {
        const stream = video.srcObject;
        const tracks = stream.getTracks();

        for (let i = 0; i < tracks.length; i++) {
            const track = tracks[i];
            track.stop();
        }
        video.srcObject = null;
    }
    // This condition will ask permission to user for Webcam access
    if (navigator.mediaDevices.getUserMedia) {
        navigator.mediaDevices.getUserMedia(constraints)
            .then(function (stream) {
                video.srcObject = stream;
                window.camera_interval = setInterval(tryTanslate, 50 );
            })
            .catch(function (err0r) {
                console.log("Something went wrong!");
            });
    }
}

