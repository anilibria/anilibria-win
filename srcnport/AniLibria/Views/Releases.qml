import QtQuick 2.12
import QtQuick.Controls 2.5
import QtQuick.Layouts 1.3
import "../Controls"

Page {
    property Drawer drawer

    background: Rectangle {
        color: "#D3D3D3"
    }

    anchors.fill: parent

    ListModel {
        id: releasesModel
        ListElement {
            name: "Jim Williams"
            portrait: "https://www.anilibria.tv/upload/release/350x500/7439.jpg?1551592833"
        }
        ListElement {
            name: "John Brown"
            portrait: "../Assets/Icons/menu.svg"
        }
        ListElement {
            name: "Bill Smyth"
            portrait: "../Assets/Icons/menu.svg"
        }
        ListElement {
            name: "Sam Wise"
            portrait: "../Assets/Icons/menu.svg"
        }
        ListElement {
            name: "Jim Williams"
            portrait: "../Assets/Icons/menu.svg"
        }
        ListElement {
            name: "John Brown"
            portrait: "../Assets/Icons/menu.svg"
        }
        ListElement {
            name: "Bill Smyth"
            portrait: "../Assets/Icons/menu.svg"
        }
        ListElement {
            name: "Sam Wise"
            portrait: "../Assets/Icons/menu.svg"
        }
        ListElement {
            name: "Jim Williams"
            portrait: "../Assets/Icons/menu.svg"
        }
        ListElement {
            name: "John Brown"
            portrait: "../Assets/Icons/menu.svg"
        }
        ListElement {
            name: "Bill Smyth"
            portrait: "../Assets/Icons/menu.svg"
        }
        ListElement {
            name: "Sam Wise"
            portrait: "../Assets/Icons/menu.svg"
        }
        ListElement {
            name: "Jim Williams"
            portrait: "../Assets/Icons/menu.svg"
        }
        ListElement {
            name: "John Brown"
            portrait: "../Assets/Icons/menu.svg"
        }
        ListElement {
            name: "Bill Smyth"
            portrait: "../Assets/Icons/menu.svg"
        }
        ListElement {
            name: "Sam Wise"
            portrait: "../Assets/Icons/menu.svg"
        }
        ListElement {
            name: "Jim Williams"
            portrait: "../Assets/Icons/menu.svg"
        }
        ListElement {
            name: "John Brown"
            portrait: "../Assets/Icons/menu.svg"
        }
        ListElement {
            name: "Bill Smyth"
            portrait: "../Assets/Icons/menu.svg"
        }
        ListElement {
            name: "Sam Wise"
            portrait: "../Assets/Icons/menu.svg"
        }
    }

    RowLayout {
        id: panelContainer
        anchors.fill: parent
        spacing: 0
        Rectangle {
            color: "#9e2323"
            width: 40
            Layout.fillHeight: true
            Column {
                AppPanelButton {
                    iconSource: "../Assets/Icons/menu.svg"
                    width: panelContainer.width
                    onPressed: {
                        drawer.open();
                    }
                }
            }
        }
        Rectangle {
            color: "#D3D3D3"
            Layout.fillHeight: true
            Layout.fillWidth: true

            ColumnLayout {
                id: gridColumn
                width: parent.width
                height: parent.height
                Rectangle {
                    Layout.fillWidth: true
                    height: 32
                    color: "#b3b3b3"
                    Switch {
                        id: selectMode
                        height: 32
                        text: "Одиночный"
                        onCheckedChanged: {
                            selectMode.text = selectMode.checked ? "Множественный" : "Одиночный";
                        }
                    }
                }

                GridView {
                    id: grid
                    Layout.alignment: Qt.AlignCenter
                    clip: true
                    snapMode: GridView.SnapToRow

                    Component {
                        id: contactsDelegate
                        Rectangle {
                            id: wrapper
                            width: grid.cellWidth - 2
                            height: grid.cellHeight - 2
                            color: "white"
                            border.color: GridView.isCurrentItem ? "#881919" : "white"
                            border.width: 2
                            MouseArea {
                                anchors.fill: parent
                                onClicked: grid.currentIndex = index
                            }
                            RowLayout {
                                anchors.fill: parent
                                Image {
                                    source: portrait
                                    fillMode: Image.PreserveAspectFit
                                    Layout.fillHeight: true
                                    Layout.preferredWidth: 210
                                    Layout.topMargin:4
                                    Layout.bottomMargin: 4
                                    Layout.leftMargin: 1
                                }
                                ColumnLayout {
                                    Layout.alignment: Qt.AlignTop
                                    Layout.fillHeight: true
                                    Layout.fillWidth: true
                                    Layout.preferredWidth: 240
                                    Layout.topMargin: 2
                                    Layout.bottomMargin: 2
                                    Text {
                                        text: name
                                    }
                                    Text {
                                        text: 'Bluherka'
                                    }
                                }
                            }
                        }
                    }
                    // The actual cell height is always as desired, but the cell width
                    // is calculated from the current width of the view and how many cells fit
                    cellHeight: 300
                    cellWidth: width / Math.floor(width / 450)
                    Layout.fillHeight: true
                    Layout.fillWidth: true
                    model: releasesModel
                    delegate: contactsDelegate
                    highlight: Rectangle { color: "lightsteelblue"; radius: 5 }
                    ScrollBar.vertical: ScrollBar {}
                }
            }
        }
    }
}
