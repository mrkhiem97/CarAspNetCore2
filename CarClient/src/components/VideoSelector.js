import React from 'react';
import uuid from 'uuid/v4';

const VideoSelector = (props) => {
    return (
        <div className="form-group">
            <label className='col-form-label'>Select video:</label>
            <select className="form-control form-control-sm" value={props.currentVideo} onChange={e => props.onVideoSelected(e.target.value)}>
                {
                    props.videos.map(element => {
                        return (
                            <option key={uuid()}>{element}</option>
                        );
                    })
                }
            </select>
        </div>
    );
}

export default VideoSelector;