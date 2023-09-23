

export class Camera {
    canvas;
    width;
    height;
    constraints;
    initialized = false;
    constructor(height, width) {
        this.height = height;
        this.width = width;
        this.constraints = {
            audio: false,
            video: {
                mandatory: {
                    minWidth: this.height,
                    minHeight: this.width
                }
            }
        }
    }

    takePhoto = () => {
        if (!this.initialized) {
            console.log("Camera not initialized.");
            return;
        }
        const context = this.canvas.getContext("2d");
        if (this.width && this.height) {
            this.canvas.width = this.width;
            this.canvas.height = this.height;
            // this will let us cut the photo down when we need to.
            context.drawImage(this.video, 0, 0, this.width, this.height, 0, 0, this.width, this.height);
            return this.canvas.toDataURL("image/png");
        }
        return null;

    }

    initialize = (elementSelector = "video#barcode", bufferSelector = "canvas#buffer") => {
        if (!navigator.mediaDevices.getUserMedia) {
            throw Error("Only desktops use should this function.");
        }
        this.video = document.querySelector(elementSelector);
        this.canvas = document.querySelector(bufferSelector);

        
        navigator.mediaDevices.getUserMedia(this.constraints)
            .then(this.handleStream)
            .catch(this.handleError);
        /* Legacy code below: getUserMedia
    else if(navigator.getUserMedia) { // Standard
    navigator.getUserMedia({ video: true }, function(stream) {
    video.src = stream;
    video.play();
    }, errBack);
    } else if(navigator.webkitGetUserMedia) { // WebKit-prefixed
    navigator.webkitGetUserMedia({ video: true }, function(stream){
    video.src = window.webkitURL.createObjectURL(stream);
    video.play();
    }, errBack);
    } else if(navigator.mozGetUserMedia) { // Mozilla-prefixed
    navigator.mozGetUserMedia({ video: true }, function(stream){
    video.srcObject = stream;
    video.play();
    }, errBack);
    }
    */
        this.initialized = true;
    }

    handleStream = (stream) => {
        this.video.srcObject = stream;
        this.video.play();
    }

    handleError = (error) => {
        console.error("Something went wrong!", error);
    }

    stop = () => {
        this.initialized = false;
        if (!this.video) return;
        var stream = this.video.srcObject;
        if (!stream) return;
        (stream.getTracks() || []).forEach(t => t.stop());
        this.video.srcObject = null;
    }
}
