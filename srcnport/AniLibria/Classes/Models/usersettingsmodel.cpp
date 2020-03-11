#include "usersettingsmodel.h"

UserSettingsModel::UserSettingsModel(): m_Quality(1),
    m_Volume(0.8),
    m_AutoNextVideo(true),
    m_AutoTopMost(false)
{

}

int UserSettingsModel::quality()
{
    return m_Quality;
}

double UserSettingsModel::volume()
{
    return m_Volume;
}

bool UserSettingsModel::autoNextVideo()
{
    return m_AutoNextVideo;
}

bool UserSettingsModel::autoTopMost()
{
    return m_AutoTopMost;
}

void UserSettingsModel::setQuality(int quality)
{
    m_Quality = quality;
}

void UserSettingsModel::setVolume(double volume)
{
    m_Volume = volume;
}

void UserSettingsModel::setAutoNextVideos(bool autoNextVideo)
{
    m_AutoNextVideo = autoNextVideo;
}

void UserSettingsModel::setAutoTopMost(bool autoTopMost)
{
    m_AutoTopMost = autoTopMost;
}

void UserSettingsModel::fromJson(QString json)
{
    if (json.isEmpty()) return;

    auto document = QJsonDocument::fromJson(json.toUtf8());
    auto jsonSettings = document.object();

    if (jsonSettings.contains("quality")) setQuality(jsonSettings.value("quality").toInt());
    if (jsonSettings.contains("volume")) setVolume(jsonSettings.value("volume").toDouble());
    if (jsonSettings.contains("autoNextVideo")) setAutoNextVideos(jsonSettings.value("autoNextVideo").toBool());
    if (jsonSettings.contains("autoTopMost")) setAutoTopMost(jsonSettings.value("autoTopMost").toBool());
}

QString UserSettingsModel::toJson()
{
    QJsonObject object;

    object["quality"] = quality();
    object["volume"] = volume();
    object["autoNextVideo"] = autoNextVideo();
    object["autoTopMost"] = autoTopMost();

    QJsonDocument saveDoc(object);
    return saveDoc.toJson();
}

