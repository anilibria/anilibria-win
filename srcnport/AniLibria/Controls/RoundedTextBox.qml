import QtQuick 2.12
import QtQuick.Controls 2.5

Rectangle {
    property string textContent: ""
    height: 20
    radius: height/2
    border.width: 1
    border.color: "#999"

    Flickable {
        id: flick
        anchors.fill: parent
        anchors.leftMargin: 6
        anchors.topMargin: 3
        anchors.rightMargin: 6
        contentWidth: edit.paintedWidth
        contentHeight: edit.paintedHeight
        clip: true

        function ensureVisible(r)
        {
            if (contentX >= r.x)
                contentX = r.x;
            else if (contentX+width <= r.x+r.width)
                contentX = r.x+r.width-width;
            if (contentY >= r.y)
                contentY = r.y;
            else if (contentY+height <= r.y+r.height)
                contentY = r.y+r.height-height;
        }

        TextEdit {
            id: edit
            width: flick.width
            focus: true
            text: textContent
            onCursorRectangleChanged: flick.ensureVisible(cursorRectangle)
        }
    }
}
