import QtQuick 2.12
import QtQuick.Controls 2.5
import QtQuick.Layouts 1.3
import Anilibria.Services 1.0

Page {
    id: page
    property Drawer drawer
    property SynchronizationService synchronizeService
    signal navigateFrom()

    background: Rectangle {
        color: "#D3D3D3"
    }

    anchors.fill: parent

    Flickable {
        id: scrollview
        anchors.fill: parent
        contentWidth: parent.width
        contentHeight: itemGrid.height
        onContentYChanged: {
            console.log(scrollview.contentY);
        }
        ScrollBar.vertical: ScrollBar {
            active: true;

            /*onActiveChanged: {
                if (!active) active = true;
            }*/
        }

        ColumnLayout {
            width: page.width
            height: page.height
            Grid {
                id: itemGrid
                Layout.alignment: Qt.AlignHCenter
                columns: 2
                spacing: 4
                //width: 540
                Repeater {
                    model: synchronizeService.releases
                    Rectangle {
                        width: 400
                        height: 200
                        radius: 10
                        color: "#f2f2f2"
                        Column {
                            Text {
                                textFormat: Text.RichText
                                color: "#a32727"
                                font.pointSize: 12
                                width: 380
                                leftPadding: 8
                                topPadding: 6
                                wrapMode: Text.WordWrap
                                maximumLineCount: 3
                                text: qsTr(modelData.title)
                            }
                            Text {
                                textFormat: Text.RichText
                                font.pointSize: 10
                                leftPadding: 8
                                topPadding: 4
                                text: qsTr("<b>Статус:</b> ") + qsTr(modelData.status)
                            }
                            Text {
                                font.pointSize: 10
                                leftPadding: 8
                                topPadding: 4
                                text: qsTr("<b>Год:</b> ") + qsTr(modelData.year)
                            }
                            Text {
                                textFormat: Text.RichText
                                font.pointSize: 10
                                leftPadding: 8
                                topPadding: 4
                                width: 380
                                wrapMode: Text.WordWrap
                                maximumLineCount: 2
                                text: qsTr("<b>Тип:</b> ") + qsTr(modelData.releaseType)
                            }
                        }
                    }
                }
            }
        }
    }

}

