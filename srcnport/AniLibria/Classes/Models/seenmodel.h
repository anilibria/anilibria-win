#ifndef SEENMODEL_H
#define SEENMODEL_H

#include <QObject>

class SeenModel
{

private:
    int m_Id;
    int m_VideoId;
    double m_VideoPosition;

public:
    int id();
    int videoId();
    double videoPosition();

    void setId(int id);
    void setVideoId(int videoId);
    void setVideoPosition(double videoPosition);

    void writeToJson(QJsonObject &json) const;
    void readFromJson(QJsonValue &json);
};

#endif // SEENMODEL_H
