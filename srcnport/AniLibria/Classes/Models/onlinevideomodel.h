#ifndef ONLINEVIDEOMODEL_H
#define ONLINEVIDEOMODEL_H

#include <QtCore>

class OnlineVideoModel
{
private:
    int m_Id;
    QString m_Title;
    QString m_SD;
    QString m_HD;
    QString m_FullHD;
    QString m_SourceSD;
    QString m_SourceHD;

public:
    OnlineVideoModel();

    void readFromApiModel(const QJsonObject &jsonObject);
};

#endif // ONLINEVIDEOMODEL_H
