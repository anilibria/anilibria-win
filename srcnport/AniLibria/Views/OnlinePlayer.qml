import QtQuick 2.12
import QtQuick.Controls 2.5
import QtMultimedia 5.12
import "../Controls"

Page {
    id: _page
    property Drawer drawer
    property bool isFullScreen: false
    property var selectedRelease: null

    signal navigateFrom()
    signal setReleaseVideo(int releaseId, int seriaOrder)
    signal changeFullScreenMode(bool fullScreen)
    onNavigateFrom: {
        player.pause();
    }
    onSetReleaseVideo: {
        const release = releasesService.getRelease(releaseId);
        if (release) _page.selectedRelease = release;
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
        onPlaybackStateChanged: {
            playButton.visible = playbackState === MediaPlayer.PausedState || playbackState === MediaPlayer.StoppedState;
            pauseButton.visible = playbackState === MediaPlayer.PlayingState;
        }
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

    Rectangle {
        id: controlPanel
        color: "white"
        anchors.bottom: parent.bottom
        width: parent.width
        height: 70
        Row {
            spacing: 5
            AppPanelButton {
                id: playButton
                iconSource: "../Assets/Icons/play-button.svg"
                width: 40
                onPressed: {
                    player.play();
                }
            }
            AppPanelButton {
                id: pauseButton
                iconSource: "../Assets/Icons/pause.svg"
                width: 40
                onPressed: {
                    player.pause();
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
}

