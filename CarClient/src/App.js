import React, { Component } from "react";

import UploadForm from "./components/UploadForm";
import ChatForm from "./components/ChatForm";
import VideoPresent from "./components/VideoPresent";

import "./App.css";

class App extends Component {
  render() {
    return (
      <div className="container">
        <div className="row app-region">
          <UploadForm />
        </div>

        <div className="row app-region">
          <div className="col-lg-6 col-md-12 col-sm-12 col-xs-12">
            <VideoPresent />
          </div>

          <div className="col-lg-6 col-md-12 col-sm-12 col-xs-12">
            <ChatForm />
          </div>
        </div>
      </div>
    );
  }
}

export default App;
