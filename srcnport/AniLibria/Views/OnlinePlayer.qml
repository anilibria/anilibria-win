import QtQuick 2.12
import QtQuick.Controls 2.5
import QtMultimedia 5.12


Page {
    property Drawer drawer
    property bool isFullScreen: false
    signal navigateFrom()
    signal setReleaseVideo(int releaseId, int seriaOrder)
    signal changeFullScreenMode(bool fullScreen)
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
            //drawer.open();
            if (player.playbackState === MediaPlayer.PlayingState) {
                player.pause();
                return;
            }
            if (player.playbackState === MediaPlayer.PausedState || player.playbackState === MediaPlayer.StoppedState) {
                player.play();
            }
        }
        onDoubleClicked: {
            isFullScreen = !isFullScreen;
            changeFullScreenMode(isFullScreen);
        }
    }
    VideoOutput {
        source: player
        anchors.fill: parent
    }

    Row {
        id: controlPanel
        anchors.bottom: parent.bottom
        spacing: 5
        //anchors.bottomMargin: buttontest.height + 10
        anchors.horizontalCenter: parent.horizontalCenter
        Button {
            id: buttontest
            text: qsTr("Пауза")
            onClicked: {
                player.pause();
            }
        }
        Button {
            text: qsTr("Воспроизведение")
            onClicked: {
                player.play();
            }
        }
        Button {
            text: qsTr("Переход вперед")
            onClicked: {
                if (player.seekable) player.seek(5000);
            }
        }
    }

}

