#include <QJsonDocument>
#include <QJsonObject>
#include <QJsonArray>
#include <QJsonValue>
#include <QHash>
#include "changesmodel.h"

ChangesModel::ChangesModel() :
    m_NewReleases(QList<int>()),
    m_NewOnlineSeries(QHash<int, int>()),
    m_NewTorrents(QHash<int,int>())
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

QHash<int, int> ChangesModel::newOnlineSeries()
{
    return m_NewOnlineSeries;
}

void ChangesModel::setNewOnlineSeries(QHash<int, int> newOnlineSeries)
{
    m_NewOnlineSeries = newOnlineSeries;
}

QHash<int, int> ChangesModel::newTorrents()
{
    return m_NewTorrents;
}

void ChangesModel::setNewTorrents(QHash<int, int> newTorrents)
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

    QJsonArray newOnlineSeries = jsonChanges.value("newOnlineSeries").toArray();

    foreach (QJsonValue newOnlineSerie, newOnlineSeries) {
        auto item = newOnlineSerie.toObject();

        m_NewOnlineSeries.insert(item.value("id").toInt(), item.value("count").toInt());
    }

    QJsonArray newTorrents = jsonChanges.value("newTorrents").toArray();

    foreach (QJsonValue newTorrent, newTorrents) {
        auto item = newTorrent.toObject();

        m_NewOnlineSeries.insert(item.value("id").toInt(), item.value("count").toInt());
    }
}

QString ChangesModel::toJson()
{
    QJsonObject object;

    QJsonArray newReleases;
    foreach (auto newRelease, m_NewReleases) {
        QJsonValue newReleaseValue(newRelease);
        newReleases.append(newReleaseValue);
    }
    object["newReleases"] = newReleases;

    QJsonArray newOnlineSeries;
    QHashIterator<int, int> newOnlineSeriesIterator(m_NewOnlineSeries);
    while (newOnlineSeriesIterator.hasNext()) {
        newOnlineSeriesIterator.next();

        QJsonObject newOnlineSerie;
        newOnlineSerie["id"] = QJsonValue(newOnlineSeriesIterator.key());
        newOnlineSerie["count"] = QJsonValue(newOnlineSeriesIterator.value());
        newOnlineSeries.append(newOnlineSerie);
    }
    object["newOnlineSeries"] = newOnlineSeries;

    QJsonArray newTorrents;
    QHashIterator<int, int> newTorrentsIterator(m_NewTorrents);
    while (newTorrentsIterator.hasNext()) {
        newTorrentsIterator.next();

        QJsonObject newTorrent;
        newTorrent["id"] = QJsonValue(newTorrentsIterator.key());
        newTorrent["count"] = QJsonValue(newTorrentsIterator.value());
        newTorrents.append(newTorrent);
    }
    object["newTorrents"] = newTorrents;

    QJsonDocument saveDoc(object);
    return saveDoc.toJson();
}
