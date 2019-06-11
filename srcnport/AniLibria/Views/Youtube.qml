import QtQuick 2.12
import QtQuick.Controls 2.5

Page {
    property Drawer drawer
    signal navigateFrom()

    background: Rectangle {
        color: "#D3D3D3"
    }

    Item {
        Text {
            text: "Text"
        }
    }
}

