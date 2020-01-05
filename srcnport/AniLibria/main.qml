import QtQuick 2.12
import QtQuick.Controls 2.5
import QtQuick.Layouts 1.3
import QtQuick.Controls.Material 2.0
import QtGraphicalEffects 1.12
import Anilibria.Services 1.0
import "Views"
import "./anilibriaService.js" as AnilibriaService

ApplicationWindow {
    id: window
    visible: true
    width: 800
    minimumWidth: 800
    minimumHeight: 600
    height: 600
    title: qsTr("AniLibria")
    property string currentPageId: "release"
    property bool synchronizationEnabled: false
    property bool notVisibleSignin: false
    property var userModel: ({})

    function showPage(pageId) {
        if (currentPageId === pageId){
            drawer.close();
            return;
        }

        const pages = {
            "videoplayer": videoplayer,
            "authorization": authorization,
            "release": releases,
            "donate": donate,
            "youtube": youtube
        };

        const currentPage = pages[currentPageId];
        currentPage.navigateFrom();

        currentPage.visible = false;

        const newPage = pages[pageId];
        newPage.visible = true;
        currentPageId = pageId;

        drawer.close();
    }

    ApplicationSettings {
        id: applicationSettings
        Component.onCompleted: {
            if (!applicationSettings.userToken) return;

            synchronizationService.getUserData(applicationSettings.userToken);
        }
    }

    LocalStorage {
        id: localStorage

        onAllReleasesFinished: {
            releases.refreshAllReleases();

            synchronizationService.synchronizeSchedule();
            if (applicationSettings.userToken) synchronizationService.synchronizeUserFavorites(applicationSettings.userToken);

            window.synchronizationEnabled = false;
        }

    }

    WorkerScript {
       id: parseReleasesWorker
       source: "parseReleases.js"
       onMessage: {
           localStorage.updateAllReleases(messageObject.releases);
       }
    }

    SynchronizationService {
        id: synchronizationService
        Component.onCompleted: {
            window.synchronizationEnabled = true;
            synchronizationService.synchronizeReleases();
        }

        onSynchronizedReleases: {
            parseReleasesWorker.sendMessage({ releasesJson: data });
        }

        onSynchronizedSchedule: {
            const jsonData = JSON.parse(data);

            if (!jsonData.status) {
                //TODO: handle error situation
            }

            const scheduleItems = jsonData.data.items;
            localStorage.setSchedule(JSON.stringify(scheduleItems));
        }

        onUserCompleteAuthentificated: {
            applicationSettings.userToken = token;
            if (window.currentPageId === "authorization") {
                showPage("release");
                synchronizationService.getUserData(applicationSettings.userToken);
            }
        }

        onUserFailedAuthentificated: {
            applicationSettings.userToken = "";
            if (!(window.currentPageId === "authorization")) return;

            authorization.authentificateFailed(errorMessage);
        }

        onUserDataReceived: {
            const userData = JSON.parse(data);
            window.userModel = userData;
            window.notVisibleSignin = true;
        }

        onUserSignouted: {
            applicationSettings.userToken = "";
            window.userModel = {};
            window.notVisibleSignin = false;
        }

        onUserFavoritesReceived:  {
            releases.refreshFavorites(JSON.parse(data));
            localStorage.updateFavorites(data);
        }

        onUserFavoritesEdited: {
            if (applicationSettings.userToken) synchronizationService.synchronizeUserFavorites(applicationSettings.userToken);
        }
    }

    Drawer {
        id: drawer
        width: 300
        height: window.height
        background:  LinearGradient {
            anchors.fill: parent
            start: Qt.point(0, 0)
            end: Qt.point(0, 300)
            gradient: Gradient {
                GradientStop { position: 0.0; color: "#6b0200" }
                GradientStop { position: 0.3; color: "#ab0909" }
                GradientStop { position: 0.5; color: "#d41515" }
                GradientStop { position: 0.7; color: "#fc2626" }
                GradientStop { position: 1.0; color: "#f52c39" }
            }
        }

        Column {            
            anchors.fill: parent

            Item {
                visible: notVisibleSignin
                width: drawer.width
                height: 64

                Image {
                    anchors.leftMargin: 6
                    anchors.topMargin: 2
                    anchors.left: parent.left
                    anchors.verticalCenter: parent.verticalCenter
                    source: userModel.avatar ? userModel.avatar : '../Assets/Icons/donate.jpg'
                    fillMode: Image.PreserveAspectCrop
                    width: 60
                    height: 60
                    layer.enabled: true
                    layer.effect: OpacityMask {
                        maskSource: Rectangle {
                            width: 60
                            height: 60
                            radius: 30
                            visible: false
                        }
                    }
                }

                Text {
                    anchors.horizontalCenter: parent.horizontalCenter
                    anchors.verticalCenter: parent.verticalCenter
                    text: userModel.login ? userModel.login : ""
                    color: "white"
                    wrapMode: Text.WordWrap
                }

                Image {
                    anchors.rightMargin: 8
                    anchors.verticalCenter: parent.verticalCenter
                    anchors.right: parent.right
                    source: "Assets/Icons/logout.svg"
                    width: 30
                    height: 30
                    MouseArea {
                        anchors.fill: parent
                        onPressed: {
                            synchronizationService.signout(applicationSettings.userToken);
                        }
                    }
                }
            }

            ItemDelegate {
                contentItem: Item {
                    Row {
                        spacing: 10
                        Image {
                            source: "Assets/Icons/menu.svg"
                            sourceSize.width: 30
                            sourceSize.height: 30
                        }
                        Label {
                            anchors.leftMargin: 10
                            anchors.verticalCenter: parent.verticalCenter
                            color: "white"
                            text: qsTr("Каталог релизов")
                        }
                    }
                }
                width: parent.width
                onClicked: {
                    showPage("release");
                }
            }
            ItemDelegate {
                contentItem: Item {
                    Row {
                        spacing: 10
                        Image {
                            source: "Assets/Icons/videoplayer.svg"
                            sourceSize.width: 30
                            sourceSize.height: 30
                        }
                        Label {
                            anchors.leftMargin: 10
                            anchors.verticalCenter: parent.verticalCenter
                            color: "white"
                            text: qsTr("Видеоплеер")
                        }
                    }
                }
                width: parent.width
                onClicked: {
                    showPage("videoplayer");
                }
            }
            ItemDelegate {
                contentItem: Item {
                    Row {
                        spacing: 10
                        Image {
                            source: "Assets/Icons/youtube.svg"
                            sourceSize.width: 30
                            sourceSize.height: 30
                        }
                        Label {
                            anchors.leftMargin: 10
                            anchors.verticalCenter: parent.verticalCenter
                            color: "white"
                            text: qsTr("Youtube")
                        }
                    }
                }
                width: parent.width
                onClicked: {
                    showPage("youtube");
                }
            }
            ItemDelegate {
                contentItem: Item {
                    Row {
                        spacing: 10
                        Image {
                            source: "Assets/Icons/donate.svg"
                            sourceSize.width: 30
                            sourceSize.height: 30
                        }
                        Label {
                            anchors.leftMargin: 10
                            anchors.verticalCenter: parent.verticalCenter
                            color: "white"
                            text: qsTr("Поддержать проект")
                        }
                    }
                }
                width: parent.width
                onClicked: {
                    showPage("donate");
                }
            }
            ItemDelegate {
                visible: !notVisibleSignin
                contentItem: Item {
                    Row {
                        spacing: 10
                        Image {
                            source: "Assets/Icons/user.svg"
                            sourceSize.width: 30
                            sourceSize.height: 30
                        }
                        Label {
                            anchors.leftMargin: 10
                            anchors.verticalCenter: parent.verticalCenter
                            color: "white"
                            text: qsTr("Войти")
                        }
                    }
                }
                width: parent.width
                onClicked: {
                    showPage("authorization");
                }
            }
        }

        Row {
            anchors.bottom: parent.bottom
            Item {
                width: 60
                height: 60
                Image {
                    anchors.verticalCenter: parent.verticalCenter
                    anchors.horizontalCenter: parent.horizontalCenter
                    source: "Assets/Icons/anilibrialogodefault.svg"
                    mipmap: true
                    width: 50
                    height: 50
                }
            }
            Item {
                width: drawer.width - 60
                height: 60
                anchors.leftMargin: 10

                Column {
                    anchors.verticalCenter: parent.verticalCenter
                    Text {
                        color: "white"
                        font.pixelSize: 14
                        text: qsTr("Клиент для сайта AniLibria")
                    }
                    Text {
                        color: "white"
                        font.pixelSize: 14
                        text: qsTr("версия 0.0.0")
                    }
                }
            }

        }
    }

    OnlinePlayer {
        id: videoplayer
        visible: false
        onChangeFullScreenMode: {
            if (fullScreen) {
                window.showFullScreen();
            } else {
                window.showNormal();
            }
        }
    }

    Releases {
        id: releases
        visible: true
        synchronizeEnabled: window.synchronizationEnabled
        onWatchRelease: {
            videoplayer.setReleaseVideo(releaseId, -1, videos);
            window.showPage("videoplayer");
        }
    }

    Authorization {
        id: authorization
        visible: false
    }

    Donate {
        id: donate
        visible: false
    }

    Youtube {
        id: youtube
        visible: false
    }
}
