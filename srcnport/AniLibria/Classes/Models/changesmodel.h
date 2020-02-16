#ifndef CHANGESMODEL_H
#define CHANGESMODEL_H

#include <QObject>
#include <QHash>

class ChangesModel
{

public:
    ChangesModel();

private:
    QList<int> m_NewReleases;
    QHash<int, int> m_NewOnlineSeries;
    QHash<int, int> m_NewTorrents;

public:
    QList<int> newReleases();
    void setNewReleases(QList<int> newReleases);

    QHash<int, int> newOnlineSeries();
    void setNewOnlineSeries(QHash<int, int> newOnlineSeries);

    QHash<int, int> newTorrents();
    void setNewTorrents(QHash<int, int> newTorrents);

    void fromJson(QString json);

    QString toJson();

};

#endif // CHANGESMODEL_H
