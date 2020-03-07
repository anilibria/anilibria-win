import QtQuick 2.12
import QtQuick.Controls 2.12
import QtQuick.Layouts 1.3
import "../Controls"

Page {
    id: youtubePage
    anchors.fill: parent

    signal navigateFrom()

    signal navigateTo()

    background: Rectangle {
        color: "#D3D3D3"
    }

    RowLayout {
        id: youtubeContainer
        anchors.fill: parent
        spacing: 0
        Rectangle {
            color: "#9e2323"
            width: 40
            Layout.fillHeight: true
            Column {
                IconButton {
                    height: 45
                    width: 40
                    iconColor: "white"
                    iconPath: "../Assets/Icons/menu.svg"
                    iconWidth: 29
                    iconHeight: 29
                    onButtonPressed: {
                        drawer.open();
                    }
                }
            }
        }
        Rectangle {
            color: "#D3D3D3"
            Layout.fillHeight: true
            Layout.fillWidth: true
            Text {
                text: qsTr("Здесь будет список youtube видео")
            }
        }
    }

}

