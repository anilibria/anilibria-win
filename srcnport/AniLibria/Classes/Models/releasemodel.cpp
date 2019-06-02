#include "releasemodel.h"
#include "onlinevideomodel.h"

ReleaseModel::ReleaseModel()
{

}

void ReleaseModel::readFromApiModel(const QJsonObject &jsonObject)
{
    m_Id = jsonObject.value("id").toInt();
    m_Code = jsonObject.value("code").toString();
    m_Poster = jsonObject.value("poster").toString();
    m_Series = jsonObject.value("series").toString();
    m_Status = jsonObject.value("status").toString();
    m_Timestamp = jsonObject.value("last").toString();
    m_Type = jsonObject.value("type").toString();
    m_Year = jsonObject.value("year").toString();
    m_Description = jsonObject.value("description").toString();
    m_IsBlocked = jsonObject.value("blockedInfo").toObject().value("blocked").toBool(false);
    m_Rating = jsonObject.value("favorite").toInt(0);
    auto names = jsonObject.value("names").toArray();
    foreach(const QJsonValue & name, names) m_Names.append(name.toString());

    auto voicers = jsonObject.value("voices").toArray();
    foreach(const QJsonValue & voicer, voicers) m_Voices.append(voicer.toString());

    auto genres = jsonObject.value("genres").toArray();
    foreach(const QJsonValue & genre, genres) m_Genres.append(genre.toString());


}

QString ReleaseModel::code()
{
    return m_Code;
}

QString ReleaseModel::series()
{
    return m_Series;
}

QString ReleaseModel::poster()
{
    return m_Poster;
}

QString ReleaseModel::timestamp()
{
    return m_Timestamp;
}

QString ReleaseModel::status()
{
    return m_Status;
}

QString ReleaseModel::type()
{
    return m_Type;
}

QString ReleaseModel::year()
{
    return m_Year;
}

QString ReleaseModel::description()
{
    return m_Description;
}

QStringList ReleaseModel::genres()
{
    return m_Genres;
}

QStringList ReleaseModel::voices()
{
    return m_Voices;
}

QStringList ReleaseModel::names()
{
    return m_Names;
}

int ReleaseModel::rating()
{
    return m_Rating;
}

bool ReleaseModel::isBlocked()
{
    return m_IsBlocked;
}
