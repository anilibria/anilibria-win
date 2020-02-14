#include <QJsonDocument>
#include <QJsonObject>
#include <QJsonArray>
#include "changesmodel.h"

ChangesModel::ChangesModel() :
    m_NewReleases(QList<int>()),
    m_NewOnlineSeries(QMap<int,int>()),
    m_NewTorrents(QMap<int,int>())
{

}

QList<int> ChangesModel::newReleases()
{
    return m_NewReleases;
}

void ChangesModel::setNewReleases(QList<int> newReleases)
{
    m_NewReleases = newReleases;
}

QMap<int, int> ChangesModel::newOnlineSeries()
{
    return m_NewOnlineSeries;
}

void ChangesModel::setNewOnlineSeries(QMap<int, int> newOnlineSeries)
{
    m_NewOnlineSeries = newOnlineSeries;
}

QMap<int, int> ChangesModel::newTorrents()
{
    return m_NewTorrents;
}

void ChangesModel::setNewTorrents(QMap<int, int> newTorrents)
{
    m_NewTorrents = newTorrents;
}

void ChangesModel::fromJson(QString json)
{
    if (json.isEmpty()) return;

    auto document = QJsonDocument::fromJson(json.toUtf8());
    auto jsonChanges = document.object();
    auto newReleases = jsonChanges.value("newReleases").toArray();

    foreach (auto newRelease, newReleases) m_NewReleases.append(newRelease.toInt());

    auto newOnlineSeries = jsonChanges.value("newOnlineSeries").toArray();

    foreach (auto newOnlineSerie, newOnlineSeries) {
        auto serieObject = newOnlineSerie.toObject();
        auto identifier = serieObject.value("id").toInt();
        auto count = serieObject.value("count").toInt();
        m_NewOnlineSeries.insert(identifier, count);
    }

    auto newTorrents = jsonChanges.value("newTorrents").toArray();

}

QString ChangesModel::toJson()
{
    return "";
}
