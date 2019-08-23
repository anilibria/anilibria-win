#ifndef RELEASESSERVICE_H
#define RELEASESSERVICE_H

#include <QObject>

class ReleasesService : public QObject
{
    Q_OBJECT
public:
    explicit ReleasesService(QObject *parent = nullptr);

signals:

public slots:
};

#endif // RELEASESSERVICE_H
