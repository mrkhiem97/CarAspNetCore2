import React from 'react';
import videojs from 'video.js';

import ApiUrl from '../api/ApiUrl';

export default class VideoPlayer extends React.Component {
    componentDidMount() {
        // instantiate Video.js
        this.player = videojs(this.videoNode, this.props.videoJsOption, function onPlayerReady() {
            console.log('onPlayerReady', this)
        });
    }

    // destroy player on unmount
    componentWillUnmount() {
        if (this.player) {
            this.player.dispose()
        }
    }

    componentWillReceiveProps(nextProps) {
        const videoUrl = `${ApiUrl.videoHost}${nextProps.currentVideo}`;
        this.player.src(videoUrl);
    }

    // wrap the player in a div with a `data-vjs-player` attribute
    // so videojs won't create additional wrapper in the DOM
    // see https://github.com/videojs/video.js/pull/3856
    render() {
        return (
            <div>
                <div data-vjs-player>
                    <video ref={node => this.videoNode = node} className="video-js vjs-default-skin vjs-big-play-centered"></video>
                </div>
            </div>
        );
    }
}