#ifndef CHANGESMODEL_H
#define CHANGESMODEL_H

#include <QObject>
#include <QMap>

class ChangesModel
{

public:
    ChangesModel();

private:
    QList<int> m_NewReleases;
    QMap<int, int> m_NewOnlineSeries;
    QMap<int, int> m_NewTorrents;

public:
    QList<int> newReleases();
    void setNewReleases(QList<int> newReleases);

    QMap<int, int> newOnlineSeries();
    void setNewOnlineSeries(QMap<int, int> newOnlineSeries);

    QMap<int, int> newTorrents();
    void setNewTorrents(QMap<int, int> newTorrents);

    void fromJson(QString json);

    QString toJson();

};

#endif // CHANGESMODEL_H
