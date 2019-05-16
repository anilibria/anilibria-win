import QtQuick 2.12
import QtQuick.Controls 2.5
import QtMultimedia 5.12


Page {
    property Drawer drawer
    signal navigateFrom()
    signal setReleaseVideo(int releaseId, int seriaOrder)
    onNavigateFrom: {
        player.pause();
    }
    onSetReleaseVideo: {
        // get seria
    }

    anchors.fill: parent

    background: Rectangle {
        color: "black"
    }

    MediaPlayer {
        id: player
        source: "https://de10.anilibria.tv/videos/ts/3996/0105-sd/playlist.m3u8"
        //source: "D:\ChromeDownload\test.mp4"
        autoPlay: false
    }

    MouseArea {
        anchors.fill: parent
        onClicked: {
            drawer.open();
        }
    }
    VideoOutput {
        source: player
        anchors.fill: parent
    }

}

