import React from 'react';
import VideoPlayer from './VideoPlayer';

const videoJsOptions = {
    autoplay: true,
    controls: true,
    sources: [{
        src: 'http://localhost:51080/api/Car/PlayVideoAsync?filename=pacific-ocean.mp4',
        type: 'video/mp4'
    }],
    playbackRates: [0.5, 1, 1.5, 2, 5, 10],
    width: 550,
    height: 250
}

const VideoPresent = () => {
    return (
        <VideoPlayer {...videoJsOptions} />
    );
}

export default VideoPresent;