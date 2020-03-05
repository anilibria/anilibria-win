#include <QJsonObject>
#include <QJsonObject>
#include <QJsonDocument>
#include "seenmodel.h"


int SeenModel::id()
{
    return m_Id;
}

int SeenModel::videoId()
{
    return m_VideoId;
}

double SeenModel::videoPosition()
{
    return m_VideoPosition;
}

void SeenModel::setId(int id)
{
    m_Id = id;
}

void SeenModel::setVideoId(int videoId)
{
    m_VideoId = videoId;
}

void SeenModel::setVideoPosition(double videoPosition)
{
    m_VideoPosition = videoPosition;
}

void SeenModel::writeToJson(QJsonObject &json) const
{
    json["id"] = m_Id;
    json["videoId"] = m_VideoId;
    json["videoPosition"] = m_VideoPosition;
}

void SeenModel::readFromJson(QJsonValue &json)
{
    setId(json["id"].toInt());
    setVideoId(json["videoId"].toInt());
    setVideoPosition(json["videoPosition"].toDouble());
}
