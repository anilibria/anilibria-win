#include "releaseitemmodel.h"

ReleaseItemModel::ReleaseItemModel(QObject *parent) : QObject(parent)
{

}

void ReleaseItemModel::mapFromReleaseModel(ReleaseModel &releaseModel)
{
    setTitle(releaseModel.title());
    setStatus(releaseModel.status());
    setYear(releaseModel.year());
    QString fullPosterUrl = "https://www.anilibria.tv" + releaseModel.poster();
    setPoster(fullPosterUrl);
    setReleaseType(releaseModel.type());
    setVoicers(releaseModel.voices().join(", "));
    setGenres(releaseModel.genres().join(", "));
}

QString ReleaseItemModel::title() const
{
    return m_Title;
}

void ReleaseItemModel::setTitle(const QString &title)
{
    if (title == m_Title) return;

    m_Title = title;
    emit titleChanged();
}

QString ReleaseItemModel::status() const
{
    return m_Status;
}

void ReleaseItemModel::setStatus(const QString &status)
{
    if (status == m_Status) return;

    m_Status = status;
    emit statusChanged();
}

QString ReleaseItemModel::year() const
{
    return m_Year;
}

void ReleaseItemModel::setYear(const QString &year)
{
    if (year == m_Year) return;

    m_Year = year;
    emit yearChanged();
}

QString ReleaseItemModel::poster() const
{
    return m_Poster;
}

void ReleaseItemModel::setPoster(const QString &poster)
{
    if (poster == m_Poster) return;

    m_Poster = poster;
    emit posterChanged();
}

QString ReleaseItemModel::description() const
{
    return m_Description;
}

void ReleaseItemModel::setDescription(const QString &description)
{
    if (description == m_Description) return;

    m_Description = description;
    emit descriptionChanged();
}

QString ReleaseItemModel::releaseType() const
{
    return m_ReleaseType;
}

void ReleaseItemModel::setReleaseType(const QString &releaseType)
{
    if (releaseType == m_ReleaseType) return;

    m_ReleaseType = releaseType;
    emit releaseTypeChanged();
}

QString ReleaseItemModel::genres() const
{
    return m_Genres;
}

void ReleaseItemModel::setGenres(const QString &genres)
{
    if (genres == m_Genres) return;

    m_Genres = genres;
    emit genresChanged();
}

QString ReleaseItemModel::voicers() const
{
    return m_Voicers;
}

void ReleaseItemModel::setVoicers(const QString &voicers)
{
    if (voicers == m_Voicers) return;

    m_Voicers = voicers;
    emit voicersChanged();
}
