import React from 'react';
import VideoPlayer from './VideoPlayer';
import VideoSelector from './VideoSelector';
import ApiUrl from '../api/ApiUrl';

import $ from 'jquery';

const videoJsOptions = {
    autoplay: false,
    controls: true,
    sources: [{
        src: `${ApiUrl}jellyfish.mp4`,
        type: 'video/mp4'.videoHost
    }
    ],
    playbackRates: [0.5, 1, 1.5, 2, 5, 10],
    width: 550,
    height: 250
}

class VideoPresent extends React.Component {
    constructor() {
        super();

        this.state = {
            currentVideo: '',
            videos: []
        }
    }

    componentDidMount() {
        $.get(ApiUrl.videoApi, (data, status) => {
            if (status === 'success') {
                this.setState({
                    videos: data,
                    currentVideo: data[0]
                });
            }
        });
    }

    onVideoSelected = (filename) => {
        this.setState(Object.assign({}, this.state, {
            currentVideo: filename
        }));
    }

    render() {
        return (
            <div>
                <VideoPlayer videoJsOption={videoJsOptions} currentVideo={this.state.currentVideo} />
                <VideoSelector videos={this.state.videos} currentVideo={this.state.currentVideo} onVideoSelected={this.onVideoSelected} />
            </div>
        );
    }
}

export default VideoPresent;