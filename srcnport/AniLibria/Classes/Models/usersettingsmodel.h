#ifndef USERSETTINGSMODEL_H
#define USERSETTINGSMODEL_H

#include <QString>
#include <QJsonDocument>
#include <QJsonObject>

class UserSettingsModel
{

private:
    int m_Quality;
    double m_Volume;
    bool m_AutoNextVideo;
    bool m_AutoTopMost;

public:
    UserSettingsModel();

    int quality();
    double volume();
    bool autoNextVideo();
    bool autoTopMost();

    void setQuality(int quality);
    void setVolume(double volume);
    void setAutoNextVideos(bool autoNextVideo);
    void setAutoTopMost(bool autoTopMost);

    void fromJson(QString json);
    QString toJson();

};

#endif // USERSETTINGSMODEL_H
